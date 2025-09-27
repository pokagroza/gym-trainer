using Application.Abstractions;
using Application.Repositories;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public interface IExternalWorkoutSyncService
{
    Task<IReadOnlyCollection<ExternalWorkoutDto>> PreviewAsync(int page = 1, int pageSize = 10, CancellationToken ct = default);
    Task<int> ImportAsync(string externalId, bool overwriteExisting = false, CancellationToken ct = default);
}

public class ExternalWorkoutSyncService : IExternalWorkoutSyncService
{
    private readonly IExternalWorkoutProvider _provider;
    private readonly IExerciseRepository _exercises;
    private readonly ITrainingSplitRepository _splits;
    private readonly Application.Abstractions.IAppDbContext _ctx;
    private readonly ILogger<ExternalWorkoutSyncService> _logger;
    private readonly IImportMetricsService? _metrics;

    public ExternalWorkoutSyncService(IExternalWorkoutProvider provider,
        IExerciseRepository exercises,
        ITrainingSplitRepository splits,
        Application.Abstractions.IAppDbContext ctx,
        ILogger<ExternalWorkoutSyncService> logger,
        IImportMetricsService? metrics = null)
    {
        _provider = provider;
        _exercises = exercises;
        _splits = splits;
        _ctx = ctx;
        _logger = logger;
        _metrics = metrics;
    }

    public async Task<IReadOnlyCollection<ExternalWorkoutDto>> PreviewAsync(int page = 1, int pageSize = 10, CancellationToken ct = default)
        => await _provider.GetWorkoutsAsync(page, pageSize, ct);

    public async Task<int> ImportAsync(string externalId, bool overwriteExisting = false, CancellationToken ct = default)
    {
        var workout = await _provider.GetWorkoutByIdAsync(externalId, ct);
        if (workout == null) return -1;

        // Simple duplicate check by name
        var existing = _splits.GetAllRaw().FirstOrDefault(s => s.ExternalId == workout.ExternalId || s.Name == workout.Title);
        if (existing != null)
        {
            _logger.LogInformation("Workout '{Title}' already exists (id={Id})", workout.Title, existing.Id);
            _metrics?.IncrementSkippedExercise();
            return existing.Id;
        }

        var ensuredExercises = new List<Exercise>();

        // Preload existing exercises into dictionaries for fast lookup
        var existingByExternal = _ctx.Exercises.Where(e => e.ExternalExerciseId != null)
            .ToDictionary(e => e.ExternalExerciseId!, e => e);
        var existingByName = _ctx.Exercises.ToDictionary(e => e.Name.Trim().ToLowerInvariant(), e => e);

        var categoryLookup = _ctx.ExerciseCategories.ToDictionary(c => c.Name.ToLowerInvariant(), c => c.Id);
        int EnsureCategory(string muscleGroup)
        {
            var normalizedCat = MuscleGroupNormalizer.Normalize(muscleGroup);
            var key = normalizedCat.ToLowerInvariant();
            if (categoryLookup.TryGetValue(key, out var existingId)) return existingId;
            var cat = new Domain.Entities.ExerciseCategory { Name = normalizedCat, Description = $"Auto-created for muscle group {normalizedCat}" };
            _ctx.ExerciseCategories.Add(cat);
            _ctx.SaveChanges();
            categoryLookup[key] = cat.Id;
            return cat.Id;
        }

        var newExercises = new List<Exercise>();
        foreach (var ee in workout.Exercises ?? Array.Empty<ExternalExerciseDto>())
        {
            Exercise? found = null;
            if (!string.IsNullOrWhiteSpace(ee.ExternalId) && existingByExternal.TryGetValue(ee.ExternalId, out var byExt))
                found = byExt;
            else
            {
                var key = ee.Name.Trim().ToLowerInvariant();
                existingByName.TryGetValue(key, out found);
            }
            var normalizedGroup = MuscleGroupNormalizer.Normalize(ee.MuscleGroup);
            var catId = EnsureCategory(normalizedGroup);
            if (found == null)
            {
                found = new Exercise
                {
                    Name = ee.Name,
                    MuscleGroup = ee.MuscleGroup,
                    NormalizedMuscleGroup = normalizedGroup,
                    Equipment = ee.Equipment ?? string.Empty,
                    Description = ee.Instructions ?? string.Empty,
                    Instructions = ee.Instructions ?? string.Empty,
                    ExerciseCategoryId = catId,
                    ExternalExerciseId = ee.ExternalId
                };
                newExercises.Add(found);
                if (!string.IsNullOrWhiteSpace(found.ExternalExerciseId))
                    existingByExternal[found.ExternalExerciseId] = found;
                existingByName[found.Name.Trim().ToLowerInvariant()] = found;
                _metrics?.IncrementNewExercise();
            }
            else if (overwriteExisting)
            {
                found.MuscleGroup = ee.MuscleGroup;
                found.NormalizedMuscleGroup = normalizedGroup;
                found.Equipment = ee.Equipment ?? found.Equipment;
                if (!string.IsNullOrWhiteSpace(ee.Instructions))
                {
                    found.Description = ee.Instructions;
                    found.Instructions = ee.Instructions;
                }
                if (found.ExerciseCategoryId == null)
                    found.ExerciseCategoryId = catId;
            }
            ensuredExercises.Add(found);
        }
        if (newExercises.Count > 0)
            _ctx.Exercises.AddRange(newExercises);
        _ctx.SaveChanges();

        var split = new TrainingSplit
        {
            Name = workout.Title,
            Description = workout.Description,
            Difficulty = Difficulty.Medium,
            ExternalId = workout.ExternalId,
            Exercises = new List<SplitExercise>()
        };

    foreach (var ex in ensuredExercises)
        {
            split.Exercises!.Add(new SplitExercise
            {
                ExerciseId = ex.Id,
                Sets = 3,
                Reps = 10
            });
        }

        _ctx.TrainingSplits.Add(split);
        _ctx.SaveChanges();
        _metrics?.IncrementNewWorkout();
        _logger.LogInformation("Imported external workout '{Title}' as split {Id}", workout.Title, split.Id);
        return split.Id;
    }
}