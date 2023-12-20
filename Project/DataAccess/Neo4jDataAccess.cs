using Neo4j.Driver;
using Project.Models;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Project.DataAccess;

public class Neo4jDataAccess : INeo4jDataAccess
{
    private IAsyncSession _session;
    private ILogger<Neo4jDataAccess> _logger;
    private readonly string _db = "neo4j";

    public Neo4jDataAccess(IDriver driver, ILogger<Neo4jDataAccess> logger)
    {
        _logger = logger;
        _session = driver.AsyncSession(o => o.WithDatabase(_db));
    }

    public async Task<List<UserModel>> GetUsers()
    {
        var query = @"MATCH (u:User) 
RETURN u.id as id,
       u.firstName + "" "" + u.lastName as name,
       u.gender as gender,
       u.dateOfBirth as dateOfBirth";
        
        return await _session.ExecuteReadAsync(async transaction =>
        {
            var cursor = await transaction.RunAsync(query);

            return await cursor.ToListAsync(record => new UserModel
            {
                Id = record["id"].As<string>(),
                Name = record["name"].As<string>(),
                Gender = record["gender"].As<string>(),
                DateOfBirth = record["dateOfBirth"].As<string>(),
            });
        });
    }

    public async Task<List<PostModel>> GetPostsByAuthor(string authorId)
    {
        var query = @"MATCH (p:Post)<-[:Authors]-(user {id: $authorId}) 
    RETURN p.id AS id,
           p.content AS content, 
           p.isComment AS isComment, 
           p.date AS date";
        return await _session.ExecuteReadAsync(async transaction =>
        {
            var cursor = await transaction.RunAsync(query,new {authorId});

            return await cursor.ToListAsync(record => new PostModel
            {
                Id = record["id"].As<string>(),
                Content = record["content"].As<string>(),
                IsComment = record["isComment"].As<bool>(),
                Date = record["date"].As<string>(),
            });
        });
    }
    
    public async Task<List<PostModel>> GetCommentsByPost(string postId)
    {
        var query = @"MATCH (p:Post {id: $postId })<-[:Comments]-(c:Post) 
    RETURN c.id AS id,
           c.content AS content, 
           c.isComment AS isComment, 
           c.date AS date";
        return await _session.ExecuteReadAsync(async transaction =>
        {
            var cursor = await transaction.RunAsync(query,new {postId});

            return await cursor.ToListAsync(record => new PostModel
            {
                Id = record["id"].As<string>(),
                Content = record["content"].As<string>(),
                IsComment = record["isComment"].As<bool>(),
                Date = record["date"].As<string>(),
            });
        });
    }

    public async Task<List<ReactionModel>> GetReactionsByPost(string postId)
    {
        var query = @"MATCH (p:Post {id: $postId })<-[r:Reaction]-(u:User) 
    RETURN r.reactionType AS type,
           r.date AS date,
           u.firstName + "" "" + u.lastName as userName";
        
        return await _session.ExecuteReadAsync(async transaction =>
        {
            var cursor = await transaction.RunAsync(query,new {postId});

            return await cursor.ToListAsync(record => new ReactionModel()
            {
                Type = record["type"].As<int>(),
                UserName = record["userName"].As<string>(),
                Date = record["date"].As<string>(),
            });
        });
    }

    public async Task AddReactionToPost(string postId, string userId)
    {
        var query =
            @"MATCH (u:User {id: $userId}), (p:Post {id: $postId})
CREATE (u)-[r:Reaction {reactionType: 1, date: $date}]->(p)";

        var parameters = new Dictionary<string, object>
        {
            { "userId", userId },
            { "postId", postId },
            { "date", DateOnly.FromDateTime(DateTime.Now).ToString() }
        };
        
        await _session.ExecuteWriteAsync(async transaction =>
        {
            var cursor = await transaction.RunAsync(query,parameters);

            await cursor.ConsumeAsync();
        });
    }

    public async Task CreateNewPost(NewPostModel model, string authorId)
    {
        var query = @"MATCH (u:User {id: $authorId})
CREATE (u)-[:Authors]->(p:Post {content:$content, date:$date,id:randomUUID(),isComment:false})";
        
        var parameters = new Dictionary<string, object>
        {
            { "authorId", authorId },
            { "content", model.Content },
            { "date", DateOnly.FromDateTime(DateTime.Now).ToString() }
        };
        
        await _session.ExecuteWriteAsync(async transaction =>
        {
            var cursor = await transaction.RunAsync(query,parameters);

            await cursor.ConsumeAsync();
        });
    }
    
    public async Task CreateNewComment(NewPostModel model, string authorId)
    {
        var query = @"MATCH (u:User {id: $authorId}), (p:Post {id: $commentedPostId})
CREATE (u)-[:Authors]->(n:Post {content:$content, date:$date,id:randomUUID(),isComment:true})-[:Comments]->(p)";
        
        var parameters = new Dictionary<string, object>
        {
            { "authorId", authorId },
            { "commentedPostId", model.CommentedPostId },
            { "content", model.Content },
            { "date", DateOnly.FromDateTime(DateTime.Now).ToString() }
        };
        
        await _session.ExecuteWriteAsync(async transaction =>
        {
            var cursor = await transaction.RunAsync(query,parameters);

            await cursor.ConsumeAsync();
        });
    }

    public async Task<Dictionary<string, string>> ExecuteReadScalarAsync(string query, IDictionary<string, object>? parameters = null)
    {
        try
        {
            parameters = parameters == null ? new Dictionary<string, object>() : parameters;

            var result = await _session.ReadTransactionAsync(async tx =>
            {
                var scalar = default(Dictionary<string, string>);
                var res = await tx.RunAsync(query, parameters);
                scalar = (await res.SingleAsync())[0].As<Dictionary<string, string>>();
                return scalar;
            });

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "There was a problem while executing database query");
            return new Dictionary<string, string>();
        }
    }

    public async Task<T> ExecuteWriteTransactionAsync<T>(string query, IDictionary<string, object>? parameters = null)
    {
        try
        {
            parameters = parameters == null ? new Dictionary<string, object>() : parameters;

            var result = await _session.WriteTransactionAsync(async tx =>
            {
                T scalar = default(T);
                var res = await tx.RunAsync(query, parameters);
                scalar = (await res.SingleAsync())[0].As<T>();
                return scalar;
            });

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "There was a problem while executing database query");
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _session.CloseAsync();
    }
}