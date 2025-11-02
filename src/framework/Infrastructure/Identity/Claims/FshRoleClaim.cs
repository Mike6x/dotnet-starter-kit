using Microsoft.AspNetCore.Identity;

namespace FSH.Framework.Infrastructure.Identity.Claims;
public class FshRoleClaim : IdentityRoleClaim<string>
{
    public string? CreatedBy { get; init; }
    public DateTimeOffset CreatedOn { get; init; }
}