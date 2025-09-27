using Application.Repositories;
using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TrainingHistoryRepository : ITrainingHistoryRepository
{
    private readonly IAppDbContext _ctx;
    public TrainingHistoryRepository(IAppDbContext ctx) => _ctx = ctx;
    public void Add(TrainingHistory history) => _ctx.TrainingHistories.Add(history);
    public IEnumerable<TrainingHistory> GetRecentForUser(int userId, int take) => _ctx.TrainingHistories.Where(h => h.UserId == userId).OrderByDescending(h => h.Date).Take(take).AsNoTracking().ToList();
    public IEnumerable<TrainingHistory> GetAllForUser(int userId) => _ctx.TrainingHistories.Where(h => h.UserId == userId).OrderByDescending(h => h.Date).AsNoTracking().ToList();
    public void Save() => _ctx.SaveChanges();
}
