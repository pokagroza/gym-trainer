using Domain.Entities;
namespace Application.Repositories;
public interface IUserAuthRepository
{
    UserAuth? GetByUsername(string username);
    UserAuth? GetByUsernameAndHash(string username, string passwordHash);
    void Add(UserAuth auth);
    void Save();
}
