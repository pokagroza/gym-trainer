using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

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
    private readonly GymDbContext _context;
    private readonly Random _rng = new();

    public TrainingOptimizerService(GymDbContext context) => _context = context;

    public Domain.Entities.TrainingSplit? GetOptimizedSplit(int userId)
    {
        var user = _context.Users.Include(u => u.Anthropometries).FirstOrDefault(u => u.Id == userId);
        if (user == null) return null;
        var candidates = GetOptimizedSplitsByParams((int)Math.Round(user.FatigueLevel), EstimateExperienceFromUser(user), null, 3);
        return candidates.FirstOrDefault();
    }

    public List<Domain.Entities.TrainingSplit> GetOptimizedSplitsByParams(int fatigue, int experience, string? difficulty, int maxSuggestions = 3)
    {
        var splits = _context.TrainingSplits
            .Include(s => s.Exercises)!.ThenInclude(se => se.Exercise)
            .AsNoTracking()
            .ToList();

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
        return PickVaried(ordered, maxSuggestions);
    }

    public Domain.Entities.TrainingSplit? GetOptimizedSplitByParams(int fatigue, int experience, string? difficulty = null)
    {
        var list = GetOptimizedSplitsByParams(fatigue, experience, difficulty, 1);
        return list.FirstOrDefault();
    }

    public List<Domain.Entities.TrainingSplit> GetAllSplits()
    {
        return _context.TrainingSplits
            .Include(s => s.Exercises)!.ThenInclude(se => se.Exercise)
            .AsNoTracking()
            .ToList();
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

        baseScore += _rng.NextDouble() * 1.5;
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
            int j = _rng.Next(i + 1);
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