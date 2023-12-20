using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.DataAccess;

namespace Project.Pages.Posts;

public class AddComment : PageModel
{
    private readonly IPostRepository _repo;

    public AddComment(IPostRepository repo)
    {
        _repo = repo;
    }
    
    public void OnGet()
    {
        
    }
}