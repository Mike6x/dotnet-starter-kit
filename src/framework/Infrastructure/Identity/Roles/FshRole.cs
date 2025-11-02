using FSH.Framework.Core.Identity.Roles;
using Microsoft.AspNetCore.Identity;

namespace FSH.Framework.Infrastructure.Identity.Roles;
public class FshRole : IdentityRole, IFshRole
{
    public string? Description { get; set; }

    public FshRole(string name, string? description = null)
        : base(name)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(name);
        Description = description;
        NormalizedName = name.ToUpperInvariant();
    }
}