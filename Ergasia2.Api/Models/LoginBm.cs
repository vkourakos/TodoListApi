using System.ComponentModel.DataAnnotations;

namespace Ergasia2.Api.Models;

public class LoginBm
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required, MinLength(8), MaxLength(30)]
    public string Password { get; set; } = null!;
}
