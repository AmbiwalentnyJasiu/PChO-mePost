using Project.Models;

namespace Project.DataAccess;

public class UserRepository : IUserRepository
{
    private INeo4jDataAccess _neo4jDataAccess;
    
    public UserRepository(INeo4jDataAccess neo4jDataAccess)
    {
        _neo4jDataAccess = neo4jDataAccess;
    }
    
    public async Task<List<UserModel>> GetUsers()
    {
        return await _neo4jDataAccess.GetUsers();
    }
}