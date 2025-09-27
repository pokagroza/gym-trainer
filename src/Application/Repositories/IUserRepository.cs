using Domain.Entities;
namespace Application.Repositories;
public interface IUserRepository
{
    User? GetById(int id);
    User? GetWithAnthropometry(int id);
    void Add(User user);
    void Save();
}
