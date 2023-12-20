using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.DataAccess;
using Project.Models;

namespace Project.Pages.Posts;

public class PostDetails : PageModel
{
    public List<PostModel> Comments { get; set; }
    public List<ReactionModel> Reactions { get; set; }
    private readonly IPostRepository _repo;

    public PostDetails(IPostRepository repo)
    {
        _repo = repo;
    }
    
    public async Task OnGet(string postId)
    {
        var comments = await _repo.GetCommentsByPostId(postId);
        var reactions = await _repo.GetReactionsByPostId(postId);

        Comments = new List<PostModel>(comments);
        Reactions = new List<ReactionModel>(reactions);
    }
}