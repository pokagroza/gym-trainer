using Domain.Entities;
namespace Application.Repositories;
public interface ISplitExerciseRepository
{
    IEnumerable<SplitExercise> GetBySplitIds(IEnumerable<int> splitIds);
    IEnumerable<int> GetExerciseIdsBySplitIds(IEnumerable<int> splitIds);
}
