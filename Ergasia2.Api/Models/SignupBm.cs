using System.ComponentModel.DataAnnotations;

namespace Ergasia2.Api.Models;

public class SignupBm
{
    [EmailAddress]
    [Required]
    public string Email { get; set; } = null!;

    [DataType(DataType.Password)]
    [Required, MinLength(8), MaxLength(30)]
    public string Password { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = null!;
}

