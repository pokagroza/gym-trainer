using Application.Repositories;
using Application.Abstractions;
using Domain.Entities;

namespace Infrastructure.Repositories;

public class UserAuthRepository : IUserAuthRepository
{
    private readonly IAppDbContext _ctx;
    public UserAuthRepository(IAppDbContext ctx) => _ctx = ctx;
    public UserAuth? GetByUsername(string username) => _ctx.UserAuths.FirstOrDefault(u => u.Username == username);
    public UserAuth? GetByUsernameAndHash(string username, string passwordHash) => _ctx.UserAuths.FirstOrDefault(u => u.Username == username && u.PasswordHash == passwordHash);
    public void Add(UserAuth auth) => _ctx.UserAuths.Add(auth);
    public void Save() => _ctx.SaveChanges();
}
