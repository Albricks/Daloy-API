using Microsoft.AspNetCore.Identity;

namespace daloy_api.Models;

public class AppUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = default!;
    public DateTime BirthDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public string? AvatarBlobName { get; set; }
}
