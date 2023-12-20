using System.ComponentModel.DataAnnotations;
using Project.Pages.Account;

namespace Project.Models;

public class RegisterModel
{
    [Required]
    [Display(Name = "User Name")]
    public string Login { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    
    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }
    
    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }
    
    [Required]
    public string Gender { get; set; }
    
    [Required]
    [Display(Name = "Date Of Birth")]
    public string DateOfBirth { get; set; }
}