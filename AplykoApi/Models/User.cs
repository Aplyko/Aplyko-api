using AplykoApi.Models;
using AplykoApi.Utils.Enums;

namespace AplykoApi;

public class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    public UserProfile? Profile { get; set; }

    public DateTime CreatedAt { get; set; }
}
