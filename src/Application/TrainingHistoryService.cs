using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public interface ITrainingHistoryService
{
    void AddHistory(int userId, int splitId, string notes);
    IEnumerable<Domain.Entities.TrainingHistory> GetUserHistory(int userId);
}

public class TrainingHistoryService : ITrainingHistoryService
{
    private readonly GymDbContext _context;
    public TrainingHistoryService(GymDbContext context) => _context = context;

    public void AddHistory(int userId, int splitId, string notes)
    {
        var history = new Domain.Entities.TrainingHistory
        {
            UserId = userId,
            TrainingSplitId = splitId,
            Date = DateTime.UtcNow,
            Notes = notes
        };
        _context.TrainingHistories.Add(history);
        _context.SaveChanges();
    }

    public IEnumerable<Domain.Entities.TrainingHistory> GetUserHistory(int userId)
    {
        return _context.TrainingHistories
            .Where(h => h.UserId == userId)
            .OrderByDescending(h => h.Date)
            .AsNoTracking()
            .ToList();
    }
}