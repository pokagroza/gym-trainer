using Application.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public interface IRecommendationService
{
    IEnumerable<Domain.Entities.Exercise> RecommendExercises(int userId);
}

public class RecommendationService : IRecommendationService
{
    private readonly ITrainingHistoryRepository _historyRepo;
    private readonly ISplitExerciseRepository _splitExercises;
    private readonly IExerciseRepository _exercises;
    private readonly Microsoft.Extensions.Logging.ILogger<RecommendationService> _logger;
    public RecommendationService(ITrainingHistoryRepository historyRepo, ISplitExerciseRepository splitExercises, IExerciseRepository exercises, Microsoft.Extensions.Logging.ILogger<RecommendationService> logger)
    {
        _historyRepo = historyRepo;
        _splitExercises = splitExercises;
        _exercises = exercises;
        _logger = logger;
    }

    public IEnumerable<Domain.Entities.Exercise> RecommendExercises(int userId)
    {
        var history = _historyRepo.GetRecentForUser(userId, 10).ToList();
        var recentSplits = history.Select(h => h.TrainingSplitId).Distinct().ToList();
        var recentExerciseIds = _splitExercises.GetExerciseIdsBySplitIds(recentSplits).ToList();
        var recentMuscles = _exercises.GetByIds(recentExerciseIds).Select(e => e.MuscleGroup).Distinct().ToHashSet();
        var rec = _exercises.GetAll().Where(e => !recentMuscles.Contains(e.MuscleGroup)).Take(5).ToList();
        _logger.LogDebug("Recommendations produced {Count} new exercises for user {UserId}", rec.Count, userId);
        return rec;
    }
}