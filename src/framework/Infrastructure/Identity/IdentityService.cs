using FSH.Framework.Core.Identity;
using FSH.Framework.Infrastructure.Identity.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace FSH.Infrastructure.Identity;

public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<FshUser> _userManager;
    private readonly SignInManager<FshUser> _signInManager;
    private readonly ILogger<IdentityService> _logger;

    public IdentityService(
        UserManager<FshUser> userManager,
        SignInManager<FshUser> signInManager,
        ILogger<IdentityService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    public async Task<(string Subject, IEnumerable<Claim> Claims)?>
        ValidateCredentialsAsync(string email, string password, string? tenant = null, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            _logger.LogWarning("Invalid login attempt for {Email}", email);
            return null;
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            _logger.LogWarning("Invalid password for {Email}", email);
            return null;
        }

        // Build user claims
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.UserName ?? user.Email!)
        };

        // Add roles as claims
        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        // Add tenant if multi-tenant setup
        if (!string.IsNullOrWhiteSpace(tenant))
            claims.Add(new("tenant", tenant));

        return (user.Id, claims);
    }

    public Task<(string Subject, IEnumerable<Claim> Claims)?>
        ValidateRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        // This would normally call a persisted refresh-token store.
        // You can plug your refresh-token repository here.
        _logger.LogInformation("Refresh token validation not yet implemented.");
        return Task.FromResult<(string, IEnumerable<Claim>)?>(null);
    }
}
