using Domain.Entities;
namespace Application.Repositories;
public interface IExerciseRepository
{
    IEnumerable<Exercise> GetByIds(IEnumerable<int> ids);
    IEnumerable<Exercise> GetAll();
}
