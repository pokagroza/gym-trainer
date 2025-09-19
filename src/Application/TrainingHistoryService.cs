using Domain.Entities;
using Infrastructure.Persistence;

namespace Application.Services
{
    public class TrainingHistoryService
    {
        private readonly GymDbContext _context;
        public TrainingHistoryService(GymDbContext context)
        {
            _context = context;
        }

        public void AddHistory(int userId, int splitId, string notes)
        {
            var history = new TrainingHistory
            {
                UserId = userId,
                TrainingSplitId = splitId,
                Date = DateTime.UtcNow,
                Notes = notes
            };
            _context.TrainingHistories.Add(history);
            _context.SaveChanges();
        }

        public IEnumerable<TrainingHistory> GetUserHistory(int userId)
        {
            return _context.TrainingHistories
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.Date)
                .ToList();
        }
    }
}