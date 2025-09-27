using Application.Repositories;
using Application.Abstractions;
using Domain.Entities;

namespace Infrastructure.Repositories;

public class SplitExerciseRepository : ISplitExerciseRepository
{
    private readonly IAppDbContext _ctx;
    public SplitExerciseRepository(IAppDbContext ctx) => _ctx = ctx;
    public IEnumerable<SplitExercise> GetBySplitIds(IEnumerable<int> splitIds) => _ctx.SplitExercises.Where(se => splitIds.Contains(se.TrainingSplitId)).ToList();
    public IEnumerable<int> GetExerciseIdsBySplitIds(IEnumerable<int> splitIds) => _ctx.SplitExercises.Where(se => splitIds.Contains(se.TrainingSplitId)).Select(se => se.ExerciseId).Distinct().ToList();
}
