using System.ComponentModel.DataAnnotations;

namespace AplykoApi.Models;

internal class UserProfile
{
    public Guid Id { get; set; }

    [StringLength(30)]
    public string FirstName { get; set; } = string.Empty;

    [StringLength(30)]
    public string LastName { get; set; } = string.Empty;

    [StringLength(30)]
    public string PhoneNumber { get; set; } = string.Empty;

    [StringLength(255)]
    public string? AvatarUrl { get; set; }
}
