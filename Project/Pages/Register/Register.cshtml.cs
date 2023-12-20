using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.DataAccess;
using Project.Models;

namespace Project.Pages.Account;

[AllowAnonymous]
public class Register : PageModel
{
    [BindProperty]
    public RegisterModel RegisterModel { get; set; }

    public string[] Genders = new[] { "Male", "Female", "Unspecified" };
    
    private readonly IAccountRepository _repo;

    public Register(IAccountRepository repo)
    {
        _repo = repo;
    }
    
    
    public void OnGet()
    {
    }
    
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var result = await _repo.TryRegister(RegisterModel);

        if (result is null) return Page();

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, result.UserName),
            new(ClaimTypes.GivenName, result.DisplayName),
            new(ClaimTypes.Sid, result.Id)
        };

        var identity = new ClaimsIdentity(claims, "MyCookieAuth");
        ClaimsPrincipal claimsPrincipal = new(identity);

        await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);

        return RedirectToPage("/Index");
    }
}