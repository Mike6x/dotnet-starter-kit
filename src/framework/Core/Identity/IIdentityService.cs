using System.Security.Claims;

namespace FSH.Framework.Core.Identity;
public interface IIdentityService
{
    /// <summary>
    /// Validates the provided user credentials and returns a unique subject ID with associated claims.
    /// </summary>
    /// <param name="email">User email or username</param>
    /// <param name="password">User password</param>
    /// <param name="tenant">Optional tenant ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Subject ID and claims, or null if invalid</returns>
    Task<(string Subject, IEnumerable<Claim> Claims)?>
        ValidateCredentialsAsync(string email, string password, string? tenant = null, CancellationToken ct = default);

    /// <summary>
    /// Validates a refresh token and returns its claims if valid.
    /// </summary>
    Task<(string Subject, IEnumerable<Claim> Claims)?>
        ValidateRefreshTokenAsync(string refreshToken, CancellationToken ct = default);
}