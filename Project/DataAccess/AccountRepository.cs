using Project.Models;

namespace Project.DataAccess;

public class AccountRepository : IAccountRepository
{
    private INeo4jDataAccess _neo4jDataAccess;
    
    public AccountRepository(INeo4jDataAccess neo4jDataAccess)
    {
        _neo4jDataAccess = neo4jDataAccess;
    }
    
    public async Task<AccountModel?> TryLogin(string userName, string password)
    {
        var query = @"MATCH (u:User) WHERE u.login = $userName AND u.password = $password
    RETURN u{UserName: u.login, DisplayName: u.firstName + "" "" + u.lastName, Id: u.id}";

        IDictionary<string, object> parameters = new Dictionary<string, object>
            { { "userName", userName }, { "password", password } };

        var response = await _neo4jDataAccess.ExecuteReadScalarAsync(query, parameters);

        AccountModel? result = null;

        if (response.Any())
        {
            result = new AccountModel
            {
                Id = response["Id"],
                DisplayName = response["DisplayName"],
                UserName = response["UserName"]
            };
        }

        return result;
    }

    public async Task<AccountModel?> TryRegister(RegisterModel registerModel)
    {
        var query = @"MATCH (u:User) WHERE u.login = $login RETURN u{UserName: u.login}";

        var parameters = new Dictionary<string, object>
            { { "login", registerModel.Login } };

        var response = await _neo4jDataAccess.ExecuteReadScalarAsync(query, parameters);

        AccountModel? result = null;
        
        if (response.Any()) return result;

        var registerQuery = @"CREATE (u:User {id: randomUUID()})
SET
    u.firstName = $firstName,
    u.lastName = $lastName,
    u.login = $login,
    u.password = $password,
    u.gender = $gender,
    u.dateOfBirth = $dateOfBirth
RETURN u{UserName: u.login, DisplayName: u.firstName +  "" "" + u.lastName, Id: u.id}";
        
        var registerParameters = new Dictionary<string, object>
        {
            { "firstName", registerModel.FirstName },
            { "lastName", registerModel.LastName },
            { "login", registerModel.Login },
            { "password", registerModel.Password },
            { "gender", registerModel.Gender },
            { "dateOfBirth", registerModel.DateOfBirth },
        };

        var registerResponse = await _neo4jDataAccess.ExecuteWriteTransactionAsync<Dictionary<string,string>>(registerQuery, registerParameters);
        
        result = new AccountModel
        {
            Id = registerResponse["Id"],
            DisplayName = registerResponse["DisplayName"],
            UserName = registerResponse["UserName"]
        };

        return result;
    }
}