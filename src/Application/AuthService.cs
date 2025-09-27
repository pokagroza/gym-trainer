using Application.Abstractions;
using Application.Repositories;
using Microsoft.EntityFrameworkCore; 
using Microsoft.Extensions.Logging;

namespace Application.Services;

public interface IAuthService
{
    bool Register(string username, string password, Domain.Entities.User user);
    Domain.Entities.User? Login(string username, string password);
}

public class AuthService : IAuthService
{
    private readonly IAppDbContext _context; // for SaveChanges
    private readonly IUserRepository _users;
    private readonly IUserAuthRepository _auths;
    private readonly Microsoft.Extensions.Logging.ILogger<AuthService> _logger;
    private readonly Application.Abstractions.IUnitOfWork _uow;
    public AuthService(IAppDbContext context, IUserRepository users, IUserAuthRepository auths, Application.Abstractions.IUnitOfWork uow, Microsoft.Extensions.Logging.ILogger<AuthService> logger)
    {
        _context = context;
        _users = users;
        _auths = auths;
        _uow = uow;
        _logger = logger;
    }

    public bool Register(string username, string password, Domain.Entities.User user)
    {
        if (_auths.GetByUsername(username) != null)
        {
            _logger.LogInformation("Registration rejected: username {Username} already exists", username);
            return false;
        }
        try
        {
            _uow.BeginAsync().GetAwaiter().GetResult();
            _users.Add(user); _users.Save();
            var auth = new Domain.Entities.UserAuth
            {
                UserId = user.Id,
                Username = username,
                PasswordHash = Domain.Entities.UserAuth.HashPassword(password)
            };
            _auths.Add(auth);
            _auths.Save();
            _uow.CommitAsync().GetAwaiter().GetResult();
            _logger.LogInformation("User {UserId} registered with username {Username}", user.Id, username);
            return true;
        }
        catch (Exception ex)
        {
            _uow.RollbackAsync().GetAwaiter().GetResult();
            _logger.LogError(ex, "Registration failed for username {Username}", username);
            throw;
        }
    }

    public Domain.Entities.User? Login(string username, string password)
    {
    var hash = Domain.Entities.UserAuth.HashPassword(password);
        var auth = _auths.GetByUsernameAndHash(username, hash);
        if (auth == null) return null;
        _logger.LogDebug("User {Username} authenticated successfully", username);
        return _users.GetById(auth.UserId);
    }
}