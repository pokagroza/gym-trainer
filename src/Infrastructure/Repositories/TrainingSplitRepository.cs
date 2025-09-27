using Application.Repositories;
using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TrainingSplitRepository : ITrainingSplitRepository
{
    private readonly IAppDbContext _ctx;
    public TrainingSplitRepository(IAppDbContext ctx) => _ctx = ctx;
    public TrainingSplit? GetById(int id) => _ctx.TrainingSplits.Include(s => s.Exercises)!.ThenInclude(se => se.Exercise).FirstOrDefault(s => s.Id == id);
    public IEnumerable<TrainingSplit> GetAllWithExercises() => _ctx.TrainingSplits.Include(s => s.Exercises)!.ThenInclude(se => se.Exercise).AsNoTracking().ToList();
    public IEnumerable<TrainingSplit> GetAllRaw() => _ctx.TrainingSplits.AsNoTracking().ToList();
}
