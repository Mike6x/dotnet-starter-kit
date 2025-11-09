using Finbuckle.MultiTenant.Abstractions;
using FSH.Framework.Core.Exceptions;
using FSH.Framework.Shared.Constants;
using FSH.Framework.Shared.Multitenancy;
using FSH.Modules.Identity.Contracts.Services;
using FSH.Modules.Identity.Features.v1.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FSH.Modules.Identity.Services;

public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<FshUser> _userManager;
    private readonly ILogger<IdentityService> _logger;
    private readonly IMultiTenantContextAccessor<AppTenantInfo>? _multiTenantContextAccessor;

    public IdentityService(
        UserManager<FshUser> userManager,
        IMultiTenantContextAccessor<AppTenantInfo>? multiTenantContextAccessor,
        ILogger<IdentityService> logger)
    {
        _userManager = userManager;
        _multiTenantContextAccessor = multiTenantContextAccessor;
        _logger = logger;
    }

    public async Task<(string Subject, IEnumerable<Claim> Claims)?>
        ValidateCredentialsAsync(string email, string password, CancellationToken ct = default)
    {
        var currentTenant = _multiTenantContextAccessor!.MultiTenantContext.TenantInfo;
        if (currentTenant == null) throw new UnauthorizedException();

        if (string.IsNullOrWhiteSpace(currentTenant.Id)
           || await _userManager.FindByEmailAsync(email.Trim().Normalize()) is not { } user
           || !await _userManager.CheckPasswordAsync(user, password))
        {
            throw new UnauthorizedException();
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedException("user is deactivated");
        }

        if (!user.EmailConfirmed)
        {
            throw new UnauthorizedException("email not confirmed");
        }

        if (currentTenant.Id != MultitenancyConstants.Root.Id)
        {
            if (!currentTenant.IsActive)
            {
                throw new UnauthorizedException($"tenant {currentTenant.Id} is deactivated");
            }

            if (DateTime.UtcNow > currentTenant.ValidUpto)
            {
                throw new UnauthorizedException($"tenant {currentTenant.Id} validity has expired");
            }
        }

        // Build user claims
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.FirstName ?? string.Empty),
            new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty),
            new(ClaimConstants.Fullname, $"{user.FirstName} {user.LastName}"),
            new(ClaimTypes.Surname, user.LastName ?? string.Empty),
            new(ClaimConstants.Tenant, _multiTenantContextAccessor!.MultiTenantContext.TenantInfo!.Id),
            new(ClaimConstants.ImageUrl, user.ImageUrl == null ? string.Empty : user.ImageUrl.ToString())
        };

        // Add roles as claims
        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

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