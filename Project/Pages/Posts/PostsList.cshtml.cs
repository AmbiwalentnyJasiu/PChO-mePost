using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.DataAccess;
using Project.Models;

namespace Project.Pages.Posts;

[Authorize]
public class PostsList : PageModel
{

    public List<PostModel> Posts { get; set; }
    private readonly IPostRepository _repo;

    public PostsList(IPostRepository repo)
    {
        _repo = repo;
    }
    
    public async Task OnGet(string id)
    {
        var postsData = new List<PostModel>();
        
        if (id is null)
        {
            var userId = User.Claims.First(c => c.Type == ClaimTypes.Sid).Value;
            postsData = await _repo.GetPostsByAuthorId(userId);

        }
        else
        {
            postsData = await _repo.GetPostsByAuthorId(id);
        }

        Posts = new List<PostModel>(postsData);
    }
}