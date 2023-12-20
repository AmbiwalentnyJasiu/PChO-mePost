using Project.Models;

namespace Project.DataAccess;

public interface IAccountRepository
{
    Task<AccountModel?> TryLogin(string userName, string password);
    Task<AccountModel?> TryRegister(RegisterModel registerModel);
}