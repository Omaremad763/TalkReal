using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;
public class User : IdentityUser<Guid>
{
    public DateTime LastSeen { get; set; }

    public bool IsOnline { get; set; }
    public string? ProfileImageUrl { get; set; }

}
