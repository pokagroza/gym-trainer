using Domain.Entities;
namespace Application.Repositories;
public interface ITrainingHistoryRepository
{
    void Add(TrainingHistory history);
    IEnumerable<TrainingHistory> GetRecentForUser(int userId, int take);
    IEnumerable<TrainingHistory> GetAllForUser(int userId);
    void Save();
}
