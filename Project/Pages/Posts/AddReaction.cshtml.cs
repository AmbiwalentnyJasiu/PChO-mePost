using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.DataAccess;

namespace Project.Pages.Posts;

public class AddReaction : PageModel
{
    private readonly IPostRepository _repo;

    public AddReaction(IPostRepository repo)
    {
        _repo = repo;
    }
    
    public async Task<IActionResult> OnGetAsync(string postId)
    {
        var userId = User.Claims.First(c => c.Type == ClaimTypes.Sid).Value;

        await _repo.AddReactionToPost(postId, userId);
        
        return RedirectToPage($"/Posts/PostDetails", new {postId});
    }
}