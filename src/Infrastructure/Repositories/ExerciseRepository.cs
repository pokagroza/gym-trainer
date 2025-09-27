using Application.Repositories;
using Application.Abstractions;
using Domain.Entities;

namespace Infrastructure.Repositories;

public class ExerciseRepository : IExerciseRepository
{
    private readonly IAppDbContext _ctx;
    public ExerciseRepository(IAppDbContext ctx) => _ctx = ctx;
    public IEnumerable<Exercise> GetByIds(IEnumerable<int> ids) => _ctx.Exercises.Where(e => ids.Contains(e.Id)).ToList();
    public IEnumerable<Exercise> GetAll() => _ctx.Exercises.ToList();
}
