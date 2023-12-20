using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.DataAccess;
using Project.Models;

namespace Project.Pages.Users;

public class UsersList : PageModel
{
    public List<UserModel> Users { get; set; }
    private readonly IUserRepository _repo;

    public UsersList(IUserRepository repo)
    {
        _repo = repo;
    }
    
    public async Task OnGetAsync()
    {
        var users = await _repo.GetUsers();

        Users = new List<UserModel>(users);
    }
}