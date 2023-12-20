using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.DataAccess;
using Project.Models;

namespace Project.Pages.Posts;

public class NewPost : PageModel
{
    [BindProperty] public NewPostModel Post { get; set; } = new();
    
    private readonly IPostRepository _repo;

    public NewPost(IPostRepository repo)
    {
        _repo = repo;
    }
    
    public async Task OnGetAsync(string commentedPostId)
    {
        Post.CommentedPostId = commentedPostId ?? "";
        if (commentedPostId != null)
        {
            Post.IsComment = true;
        }
        else
        {
            Post.IsComment = false;
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        
        var userId = User.Claims.First(c => c.Type == ClaimTypes.Sid).Value;

        await _repo.CreateNewPost(Post, userId);
        
        return RedirectToPage("/Index");
    }
}