using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class TrainingOptimizerService
    {
        private readonly GymDbContext _context;
        private readonly Random _rng = new Random();
        public TrainingOptimizerService(GymDbContext context)
        {
            _context = context;
        }

        // Returns a single best split for a user by id (keeps existing contract)
        public TrainingSplit? GetOptimizedSplit(int userId)
        {
            var user = _context.Users.Include(u => u.Anthropometries).FirstOrDefault(u => u.Id == userId);
            if (user == null) return null;

            // Prefer a selection from multiple candidates for variety
            var candidates = GetOptimizedSplitsByParams((int)Math.Round(user.FatigueLevel), EstimateExperienceFromUser(user), null, 3);
            return candidates.FirstOrDefault();
        }

        // New: return up to N varied split suggestions based on parameters
        public List<TrainingSplit> GetOptimizedSplitsByParams(int fatigue, int experience, string? difficulty, int maxSuggestions = 3)
        {
            var splits = _context.TrainingSplits
                .Include(s => s.Exercises).ThenInclude(se => se.Exercise)
                .ToList();

            // If difficulty explicitly requested, filter by it first
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

            // Score splits by matching difficulty and by intensity vs fatigue/experience
            var scored = splits.Select(s => new
            {
                Split = s,
                Score = ScoreForSplit(s, fatigue, experience)
            }).OrderByDescending(x => x.Score).ToList();

            var ordered = scored.Select(x => x.Split).ToList();
            return PickVaried(ordered, maxSuggestions);
        }

        // Compatibility wrapper for older call sites that expect a single split
        public TrainingSplit? GetOptimizedSplitByParams(int fatigue, int experience, string? difficulty = null)
        {
            var list = GetOptimizedSplitsByParams(fatigue, experience, difficulty, 1);
            return list.FirstOrDefault();
        }

        public List<TrainingSplit> GetAllSplits()
        {
            return _context.TrainingSplits.Include(s => s.Exercises).ThenInclude(se => se.Exercise).ToList();
        }

        // Helper: simple heuristic to estimate experience from stored user fields (default fallback)
        private int EstimateExperienceFromUser(Domain.Entities.User user)
        {
            // If user has no explicit experience, approximate from age: older users likely have more years (very rough)
            if (user.Age <= 20) return 1;
            if (user.Age <= 30) return 2;
            if (user.Age <= 45) return 3;
            return 4;
        }

        // Score a split by how well it fits the user's fatigue and experience
        private double ScoreForSplit(Domain.Entities.TrainingSplit s, int fatigue, int experience)
        {
            double baseScore = 0;
            // Map Difficulty to numeric intensity
            int intensity = s.Difficulty switch
            {
                Domain.Entities.Difficulty.Easy => 1,
                Domain.Entities.Difficulty.Medium => 2,
                Domain.Entities.Difficulty.Hard => 3,
                _ => 2
            };

            // Favor lower intensity when fatigue is high, and higher intensity when fatigue is low and experience is high
            int desiredIntensity = 2; // neutral
            if (fatigue >= 8) desiredIntensity = 1;
            else if (fatigue <= 2 && experience >= 3) desiredIntensity = 3;

            // Score by closeness to desired intensity
            baseScore += 10 - Math.Abs(intensity - desiredIntensity) * 3;

            // Slightly prefer splits with more exercises (diversity) for experienced users
            int exerciseCount = s.Exercises?.Count ?? 0;
            baseScore += Math.Min(5, exerciseCount) * (experience >= 3 ? 1.2 : 0.6);

            // Small random jitter to introduce variety
            baseScore += _rng.NextDouble() * 1.5;

            return baseScore;
        }

        // Pick up to N splits from ordered list but shuffle nearby items to increase variety
        private List<TrainingSplit> PickVaried(List<TrainingSplit> ordered, int n)
        {
            var result = new List<TrainingSplit>();
            if (ordered == null || ordered.Count == 0) return result;

            // Take the top K, but shuffle within the top window to produce different suggestions
            int window = Math.Min(10, ordered.Count);
            var topWindow = ordered.Take(window).ToList();
            // shuffle topWindow in-place
            for (int i = topWindow.Count - 1; i > 0; i--)
            {
                int j = _rng.Next(i + 1);
                var tmp = topWindow[i];
                topWindow[i] = topWindow[j];
                topWindow[j] = tmp;
            }

            foreach (var s in topWindow)
            {
                result.Add(s);
                if (result.Count >= n) break;
            }

            return result;
        }
    }
}