using FSH.Framework.Core.Identity.Users;
using Microsoft.AspNetCore.Identity;

namespace FSH.Framework.Infrastructure.Identity.Users;
public class FshUser : IdentityUser, IFshUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public Uri? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
}
