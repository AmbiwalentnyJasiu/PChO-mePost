using Project.Models;

namespace Project.DataAccess;

public class PostRepository : IPostRepository
{
    private INeo4jDataAccess _neo4jDataAccess;

    public PostRepository(INeo4jDataAccess neo4jDataAccess)
    {
        _neo4jDataAccess = neo4jDataAccess;
    }
    
    public async Task<List<PostModel>> GetPostsByAuthorId(string id)
    { 
        var response = await _neo4jDataAccess.GetPostsByAuthor(id);

        return response;
    }
    
    public async Task<List<PostModel>> GetCommentsByPostId(string id)
    { 
        var response = await _neo4jDataAccess.GetCommentsByPost(id);

        return response;
    }

    public async Task<List<ReactionModel>> GetReactionsByPostId(string postId)
    {
        var response = await _neo4jDataAccess.GetReactionsByPost(postId);

        return response;
    }

    public async Task AddReactionToPost(string postId, string userId)
    {
        await _neo4jDataAccess.AddReactionToPost(postId, userId);
    }

    public async Task CreateNewPost(NewPostModel model, string authorId)
    {
        if (string.IsNullOrEmpty(model.CommentedPostId))
        {
            await _neo4jDataAccess.CreateNewComment(model, authorId);
        }
        
        await _neo4jDataAccess.CreateNewPost(model, authorId);
    }
}