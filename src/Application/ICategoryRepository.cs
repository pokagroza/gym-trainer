using Domain.Entities;

namespace Application.Repositories;

public interface ICategoryRepository
{
    IEnumerable<ExerciseCategory> GetAll();
    ExerciseCategory? GetById(int id);
    void Add(ExerciseCategory category);
    void Update(ExerciseCategory category);
    void Delete(int id);
    void Save();
}
