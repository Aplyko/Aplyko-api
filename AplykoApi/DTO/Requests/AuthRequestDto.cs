using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AplykoApi.DTO;

public class LoginRequestDto
{
    [Required(ErrorMessage = "The Email field is required.")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "The Password field is required.")]
    public string Password { get; set; } = string.Empty;

    [DefaultValue(false)]
    public bool RememberMe { get; set; }
}

public class RegisterRequestDto
{
    [Required(ErrorMessage = "The Email field is required.")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
    [MaxLength(128, ErrorMessage = "Password cannot exceed 128 characters.")]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character."
    )]
    [Required(ErrorMessage = "The Password field is required.")]
    public string Password { get; set; } = string.Empty;
}
