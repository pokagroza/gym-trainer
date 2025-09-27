using Domain.Entities;
namespace Application.Repositories;
public interface ITrainingSplitRepository
{
    TrainingSplit? GetById(int id);
    IEnumerable<TrainingSplit> GetAllWithExercises();
    IEnumerable<TrainingSplit> GetAllRaw();
}
