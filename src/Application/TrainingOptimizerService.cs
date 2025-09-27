using Application.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public interface ITrainingOptimizerService
{
    Domain.Entities.TrainingSplit? GetOptimizedSplit(int userId);
    List<Domain.Entities.TrainingSplit> GetOptimizedSplitsByParams(int fatigue, int experience, string? difficulty, int maxSuggestions = 3);
    Domain.Entities.TrainingSplit? GetOptimizedSplitByParams(int fatigue, int experience, string? difficulty = null);
    List<Domain.Entities.TrainingSplit> GetAllSplits();
}

public class TrainingOptimizerService : ITrainingOptimizerService
{
    private readonly IUserRepository _users;
    private readonly ITrainingSplitRepository _splits;
    private readonly Application.Abstractions.IRandomProvider _rng;
    private readonly Microsoft.Extensions.Logging.ILogger<TrainingOptimizerService> _logger;

    public TrainingOptimizerService(IUserRepository users, ITrainingSplitRepository splits, Application.Abstractions.IRandomProvider rng, Microsoft.Extensions.Logging.ILogger<TrainingOptimizerService> logger)
    {
        _users = users;
        _splits = splits;
        _rng = rng;
        _logger = logger;
    }

    public Domain.Entities.TrainingSplit? GetOptimizedSplit(int userId)
    {
        var user = _users.GetWithAnthropometry(userId);
        if (user == null) return null;
        var fatigue = (int)Math.Round(user.FatigueLevel);
        var exp = EstimateExperienceFromUser(user);
        _logger.LogDebug("Optimizing split for user {UserId} fatigue={Fatigue} exp={Exp}", userId, fatigue, exp);
        var candidates = GetOptimizedSplitsByParams(fatigue, exp, null, 3);
        return candidates.FirstOrDefault();
    }

    public List<Domain.Entities.TrainingSplit> GetOptimizedSplitsByParams(int fatigue, int experience, string? difficulty, int maxSuggestions = 3)
    {
        var splits = _splits.GetAllWithExercises().ToList();

        if (!string.IsNullOrEmpty(difficulty))
        {
            Domain.Entities.Difficulty? diff = difficulty.ToLowerInvariant() switch
            {
                "easy" => Domain.Entities.Difficulty.Easy,
                "medium" => Domain.Entities.Difficulty.Medium,
                "hard" => Domain.Entities.Difficulty.Hard,
                _ => null
            };
            if (diff.HasValue)
            {
                var byDiff = splits.Where(s => s.Difficulty == diff.Value).ToList();
                return PickVaried(byDiff, maxSuggestions);
            }
        }

        var scored = splits.Select(s => new
        {
            Split = s,
            Score = ScoreForSplit(s, fatigue, experience)
        }).OrderByDescending(x => x.Score).ToList();

        var ordered = scored.Select(x => x.Split).ToList();
        var picked = PickVaried(ordered, maxSuggestions);
        _logger.LogDebug("Optimizer produced {Count} suggestions (requested {Req})", picked.Count, maxSuggestions);
        return picked;
    }

    public Domain.Entities.TrainingSplit? GetOptimizedSplitByParams(int fatigue, int experience, string? difficulty = null)
    {
        var list = GetOptimizedSplitsByParams(fatigue, experience, difficulty, 1);
        return list.FirstOrDefault();
    }

    public List<Domain.Entities.TrainingSplit> GetAllSplits()
    {
    return _splits.GetAllWithExercises().ToList();
    }

    private int EstimateExperienceFromUser(Domain.Entities.User user)
    {
        if (user.Age <= 20) return 1;
        if (user.Age <= 30) return 2;
        if (user.Age <= 45) return 3;
        return 4;
    }

    private double ScoreForSplit(Domain.Entities.TrainingSplit s, int fatigue, int experience)
    {
        double baseScore = 0;
        int intensity = s.Difficulty switch
        {
            Domain.Entities.Difficulty.Easy => 1,
            Domain.Entities.Difficulty.Medium => 2,
            Domain.Entities.Difficulty.Hard => 3,
            _ => 2
        };

        int desiredIntensity = 2;
        if (fatigue >= 8) desiredIntensity = 1;
        else if (fatigue <= 2 && experience >= 3) desiredIntensity = 3;

        baseScore += 10 - Math.Abs(intensity - desiredIntensity) * 3;

        int exerciseCount = s.Exercises?.Count ?? 0;
        baseScore += Math.Min(5, exerciseCount) * (experience >= 3 ? 1.2 : 0.6);

        baseScore += _rng.NextDouble() * 1.5; // minor randomness
        _logger.LogTrace("Score for split {SplitId}: {Score}", s.Id, baseScore);
        return baseScore;
    }

    private List<Domain.Entities.TrainingSplit> PickVaried(List<Domain.Entities.TrainingSplit> ordered, int n)
    {
        var result = new List<Domain.Entities.TrainingSplit>();
        if (ordered == null || ordered.Count == 0) return result;
        int window = Math.Min(10, ordered.Count);
        var topWindow = ordered.Take(window).ToList();
        for (int i = topWindow.Count - 1; i > 0; i--)
        {
            int j = _rng.Next(0, i + 1);
            (topWindow[i], topWindow[j]) = (topWindow[j], topWindow[i]);
        }
        foreach (var s in topWindow)
        {
            result.Add(s);
            if (result.Count >= n) break;
        }
        return result;
    }
}