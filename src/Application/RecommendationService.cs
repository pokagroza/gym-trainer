using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public interface IRecommendationService
{
    IEnumerable<Domain.Entities.Exercise> RecommendExercises(int userId);
}

public class RecommendationService : IRecommendationService
{
    private readonly GymDbContext _context;
    public RecommendationService(GymDbContext context) => _context = context;

    public IEnumerable<Domain.Entities.Exercise> RecommendExercises(int userId)
    {
        var history = _context.TrainingHistories
            .Where(h => h.UserId == userId)
            .OrderByDescending(h => h.Date)
            .Take(10)
            .AsNoTracking()
            .ToList();
        var recentSplits = history.Select(h => h.TrainingSplitId).ToList();
        var recentExercises = _context.SplitExercises
            .Where(se => recentSplits.Contains(se.TrainingSplitId))
            .Select(se => se.ExerciseId)
            .ToList();
        var recentMuscles = _context.Exercises
            .Where(e => recentExercises.Contains(e.Id))
            .Select(e => e.MuscleGroup)
            .Distinct()
            .ToList();
        return _context.Exercises
            .Where(e => !recentMuscles.Contains(e.MuscleGroup))
            .Take(5)
            .AsNoTracking()
            .ToList();
    }
}