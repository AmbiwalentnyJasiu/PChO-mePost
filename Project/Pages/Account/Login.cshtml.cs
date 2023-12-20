using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.DataAccess;

namespace Project.Pages.Account;

public class Login : PageModel
{
    [BindProperty]
    public Credential Credential { get; set; }

    private readonly IAccountRepository _repo;

    public Login(IAccountRepository repo)
    {
        _repo = repo;
    }
    
    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var result = await _repo.TryLogin(Credential.UserName, Credential.Password);

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

public class Credential
{
    [Required]
    [Display(Name = "User Name")]
    public string UserName { get; set; }
    
    [Required]
    [Display(Name = "Password")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}