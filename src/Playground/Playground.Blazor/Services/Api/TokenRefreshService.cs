using FSH.Playground.Blazor.ApiClient;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FSH.Playground.Blazor.Services.Api;

/// <summary>
/// Service responsible for refreshing expired access tokens using the refresh token.
/// This service handles the token refresh flow when a 401 response is received.
/// </summary>
internal interface ITokenRefreshService
{
    /// <summary>
    /// Attempts to refresh the access token using the stored refresh token.
    /// </summary>
    /// <returns>The new access token if refresh succeeded, null otherwise.</returns>
    Task<string?> TryRefreshTokenAsync(CancellationToken cancellationToken = default);
}

internal sealed class TokenRefreshService : ITokenRefreshService, IDisposable
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITokenClient _tokenClient;
    private readonly ILogger<TokenRefreshService> _logger;
    private readonly SemaphoreSlim _refreshLock = new(1, 1);

    public TokenRefreshService(
        IHttpContextAccessor httpContextAccessor,
        ITokenClient tokenClient,
        ILogger<TokenRefreshService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _tokenClient = tokenClient;
        _logger = logger;
    }

    public async Task<string?> TryRefreshTokenAsync(CancellationToken cancellationToken = default)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            _logger.LogWarning("HttpContext is not available for token refresh");
            return null;
        }

        // Prevent concurrent refresh attempts
        if (!await _refreshLock.WaitAsync(TimeSpan.FromSeconds(10), cancellationToken))
        {
            _logger.LogWarning("Token refresh lock acquisition timed out");
            return null;
        }

        try
        {
            var user = httpContext.User;
            if (user?.Identity?.IsAuthenticated != true)
            {
                _logger.LogDebug("User is not authenticated, cannot refresh token");
                return null;
            }

            var currentAccessToken = user.FindFirst("access_token")?.Value;
            var refreshToken = user.FindFirst("refresh_token")?.Value;
            var tenant = user.FindFirst("tenant")?.Value ?? "root";

            if (string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogWarning("No refresh token available");
                return null;
            }

            if (string.IsNullOrEmpty(currentAccessToken))
            {
                _logger.LogWarning("No access token available for refresh");
                return null;
            }

            _logger.LogInformation(
                "Attempting to refresh access token for tenant {Tenant}. RefreshToken length: {RefreshTokenLength}, First chars: {RefreshTokenPreview}",
                tenant,
                refreshToken.Length,
                refreshToken[..Math.Min(8, refreshToken.Length)] + "...");

            // Call the refresh token API
            var refreshResponse = await _tokenClient.RefreshAsync(
                tenant,
                new RefreshTokenCommand
                {
                    Token = currentAccessToken,
                    RefreshToken = refreshToken
                },
                cancellationToken);

            if (refreshResponse is null || string.IsNullOrEmpty(refreshResponse.Token))
            {
                _logger.LogWarning("Token refresh returned empty response");
                return null;
            }

            // Parse the new JWT to extract claims
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtHandler.ReadJwtToken(refreshResponse.Token);

            // Build new claims list with updated tokens
            var newClaims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, jwtToken.Subject ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.NewGuid().ToString()),
                new(ClaimTypes.Email, user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty),
                new("access_token", refreshResponse.Token),
                new("refresh_token", refreshResponse.RefreshToken),
                new("tenant", tenant),
            };

            // Preserve name claim
            var nameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "name" || c.Type == ClaimTypes.Name);
            if (nameClaim != null)
            {
                newClaims.Add(new Claim(ClaimTypes.Name, nameClaim.Value));
            }

            // Preserve role claims
            var roleClaims = jwtToken.Claims.Where(c => c.Type == "role" || c.Type == ClaimTypes.Role);
            newClaims.AddRange(roleClaims.Select(r => new Claim(ClaimTypes.Role, r.Value)));

            // Re-sign in with updated claims
            var identity = new ClaimsIdentity(newClaims, "Cookies");
            var principal = new ClaimsPrincipal(identity);

            await httpContext.SignInAsync("Cookies", principal, new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            });

            _logger.LogInformation("Access token refreshed successfully");

            return refreshResponse.Token;
        }
        catch (ApiException ex) when (ex.StatusCode == 401)
        {
            _logger.LogWarning(ex, "Refresh token is invalid or expired, user needs to re-authenticate");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh access token");
            return null;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    public void Dispose()
    {
        _refreshLock.Dispose();
    }
}
