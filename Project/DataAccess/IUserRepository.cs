using Project.Models;

namespace Project.DataAccess;

public interface IUserRepository
{
    Task<List<UserModel>> GetUsers();
}