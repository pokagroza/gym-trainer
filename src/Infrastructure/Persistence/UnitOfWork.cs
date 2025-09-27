using Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly GymDbContext _ctx;
    private IDbContextTransaction? _tx;
    public UnitOfWork(GymDbContext ctx) { _ctx = ctx; }
    public async Task BeginAsync(CancellationToken ct = default)
    {
        if (_tx != null) return; // already started
        _tx = await _ctx.Database.BeginTransactionAsync(ct);
    }
    public async Task CommitAsync(CancellationToken ct = default)
    {
        if (_tx == null) return;
        await _tx.CommitAsync(ct);
        await _tx.DisposeAsync();
        _tx = null;
    }
    public async Task RollbackAsync(CancellationToken ct = default)
    {
        if (_tx == null) return;
        await _tx.RollbackAsync(ct);
        await _tx.DisposeAsync();
        _tx = null;
    }
    public void Dispose()
    {
        _tx?.Dispose();
    }
}