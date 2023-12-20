using Project.Models;

namespace Project.DataAccess;

public interface IPostRepository
{
    Task<List<PostModel>> GetPostsByAuthorId(string id);
    Task<List<PostModel>> GetCommentsByPostId(string id);
    Task<List<ReactionModel>> GetReactionsByPostId(string id);
    Task AddReactionToPost(string postId, string userId);
    Task CreateNewPost(NewPostModel model, string authorId);
}