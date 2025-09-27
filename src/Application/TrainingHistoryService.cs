using Application.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public interface ITrainingHistoryService
{
    void AddHistory(int userId, int splitId, string notes);
    IEnumerable<Domain.Entities.TrainingHistory> GetUserHistory(int userId);
}

public class TrainingHistoryService : ITrainingHistoryService
{
    private readonly ITrainingHistoryRepository _repo;
    private readonly Microsoft.Extensions.Logging.ILogger<TrainingHistoryService> _logger;
    public TrainingHistoryService(ITrainingHistoryRepository repo, Microsoft.Extensions.Logging.ILogger<TrainingHistoryService> logger)
    { _repo = repo; _logger = logger; }

    public void AddHistory(int userId, int splitId, string notes)
    {
        var history = new Domain.Entities.TrainingHistory
        {
            UserId = userId,
            TrainingSplitId = splitId,
            Date = DateTime.UtcNow,
            Notes = notes
        };
        _repo.Add(history);
        _repo.Save();
        _logger.LogInformation("Added training history entry for user {UserId} split {SplitId}", userId, splitId);
    }

    public IEnumerable<Domain.Entities.TrainingHistory> GetUserHistory(int userId)
    {
        return _repo.GetAllForUser(userId);
    }
}