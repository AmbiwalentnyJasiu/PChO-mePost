using Project.Models;

namespace Project.DataAccess;

public interface INeo4jDataAccess : IAsyncDisposable
{
    Task<List<UserModel>> GetUsers();
    Task<List<PostModel>> GetPostsByAuthor(string authorId);
    Task<List<PostModel>> GetCommentsByPost(string postId);
    Task<List<ReactionModel>> GetReactionsByPost(string postId);
    Task AddReactionToPost(string postId, string userId);
    Task CreateNewPost(NewPostModel model, string authorId);
    Task CreateNewComment(NewPostModel model, string authorId);
    Task<Dictionary<string, string>> ExecuteReadScalarAsync(string query, IDictionary<string, object>? parameters = null);
    Task<T> ExecuteWriteTransactionAsync<T>(string query, IDictionary<string, object>? parameters = null);
}