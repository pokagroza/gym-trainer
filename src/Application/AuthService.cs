using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore; // for transactions (Database) if needed

namespace Application.Services;

public interface IAuthService
{
    bool Register(string username, string password, Domain.Entities.User user);
    Domain.Entities.User? Login(string username, string password);
}

public class AuthService : IAuthService
{
    private readonly GymDbContext _context;
    public AuthService(GymDbContext context) => _context = context;

    public bool Register(string username, string password, Domain.Entities.User user)
    {
        if (_context.UserAuths.Any(u => u.Username == username))
            return false;
        using var transaction = _context.Database.BeginTransaction();
        try
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            var auth = new Domain.Entities.UserAuth
            {
                UserId = user.Id,
                Username = username,
                PasswordHash = Domain.Entities.UserAuth.HashPassword(password)
            };
            _context.UserAuths.Add(auth);
            _context.SaveChanges();
            transaction.Commit();
            return true;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public Domain.Entities.User? Login(string username, string password)
    {
    var hash = Domain.Entities.UserAuth.HashPassword(password);
        var auth = _context.UserAuths.FirstOrDefault(u => u.Username == username && u.PasswordHash == hash);
        if (auth == null) return null;
        return _context.Users.FirstOrDefault(u => u.Id == auth.UserId);
    }
}