using Domain.Entities;
using Infrastructure.Persistence;

namespace Application.Services
{
    public class RecommendationService
    {
        private readonly GymDbContext _context;
        public RecommendationService(GymDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Exercise> RecommendExercises(int userId)
        {
            // Пример: если пользователь часто тренирует одну группу мышц — рекомендовать другие
            var history = _context.TrainingHistories
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.Date)
                .Take(10)
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
            // Рекомендовать упражнения на другие группы мышц
            return _context.Exercises
                .Where(e => !recentMuscles.Contains(e.MuscleGroup))
                .Take(5)
                .ToList();
        }
    }
}