using Domain.Entities;
using Infrastructure.Persistence;

namespace Application.Services
{
    public class AuthService
    {
        private readonly GymDbContext _context;
        public AuthService(GymDbContext context)
        {
            _context = context;
        }

        public bool Register(string username, string password, User user)
        {
            if (_context.UserAuths.Any(u => u.Username == username))
                return false;
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                var auth = new UserAuth
                {
                    UserId = user.Id,
                    Username = username,
                    PasswordHash = UserAuth.HashPassword(password)
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

        public User? Login(string username, string password)
        {
            var hash = UserAuth.HashPassword(password);
            var auth = _context.UserAuths.FirstOrDefault(u => u.Username == username && u.PasswordHash == hash);
            if (auth == null) return null;
            return _context.Users.FirstOrDefault(u => u.Id == auth.UserId);
        }
    }
}