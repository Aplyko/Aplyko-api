using AplykoApi.Utils.Enums;

namespace AplykoApi.DTO;

internal class CreateUserDto
{
    public string Email { get; set; } = string.Empty;
    public string? Password { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}

internal class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Password { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
}
