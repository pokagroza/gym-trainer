using Application.Repositories;
using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IAppDbContext _ctx;
    public UserRepository(IAppDbContext ctx) => _ctx = ctx;
    public User? GetById(int id) => _ctx.Users.FirstOrDefault(u => u.Id == id);
    public User? GetWithAnthropometry(int id) => _ctx.Users.Include(u => u.Anthropometries).FirstOrDefault(u => u.Id == id);
    public void Add(User user) => _ctx.Users.Add(user);
    public void Save() => _ctx.SaveChanges();
}
