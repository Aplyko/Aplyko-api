using System.ComponentModel.DataAnnotations;

namespace AplykoApi.DTO;

public class UserRequestDto
{
}

public class EditPasswordRequestDto
{
    [Required(ErrorMessage = "The old password is required.")]
    public string OldPassword { get; set; } = string.Empty;

    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
    [MaxLength(128, ErrorMessage = "Password cannot exceed 128 characters.")]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$",
        ErrorMessage =
            "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character."
    )]
    [Required(ErrorMessage = "The new password is required.")]
    public string NewPassword { get; set; } = string.Empty;
}
