using System.ComponentModel.DataAnnotations;

namespace SecureMailApp.Models;

public class LoginViewModel
{
    [Required]
    [Display(Name = "Username or Email")]
    public string Identifier { get; set; } = null!;   

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = null!;
}
