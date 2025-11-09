using System.Security.Claims;

namespace FSH.Modules.Identity.Contracts.DTOs;
public interface ITokenService
{
    /// <summary>
    /// Issues a new access and refresh token for the specified subject.
    /// </summary>
    Task<TokenResponse> IssueAsync(
        string subject,
        IEnumerable<Claim> claims,
        string? tenant = null,
        CancellationToken ct = default);
}