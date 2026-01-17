using FSH.Playground.Blazor.ApiClient;
using Microsoft.AspNetCore.Authentication;
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
    private readonly ICircuitTokenCache _circuitTokenCache;
    private readonly ILogger<TokenRefreshService> _logger;

    // Static lock and cache shared across all scoped instances
    // This is critical because the service is registered as Scoped,
    // but we need to coordinate across all concurrent requests
    private static readonly SemaphoreSlim RefreshLock = new(1, 1);

    // Cache the last refreshed token to prevent race conditions
    // When multiple concurrent requests detect token expiration, only the first
    // should actually refresh - others should use the cached result
    private static string? _lastRefreshedToken;
    private static string? _cachedForRefreshToken; // The refresh token we used to get the cached access token
    private static DateTime _lastRefreshTime = DateTime.MinValue;
    private static readonly TimeSpan RefreshCacheDuration = TimeSpan.FromSeconds(30);

    // Track failed refresh tokens to prevent endless retry loops
    // When a refresh token fails with 401, we mark it as failed so we don't keep retrying
    private static string? _failedRefreshToken;
    private static DateTime _failedRefreshTime = DateTime.MinValue;
    private static readonly TimeSpan FailedTokenCacheDuration = TimeSpan.FromMinutes(5);

    public TokenRefreshService(
        IHttpContextAccessor httpContextAccessor,
        ITokenClient tokenClient,
        ICircuitTokenCache circuitTokenCache,
        ILogger<TokenRefreshService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _tokenClient = tokenClient;
        _circuitTokenCache = circuitTokenCache;
        _logger = logger;
    }

    public async Task<string?> TryRefreshTokenAsync(CancellationToken cancellationToken = default)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            _logger.LogDebug("HttpContext is not available for token refresh");
            return null;
        }

        // Get current refresh token - check circuit cache first, then fall back to claims
        // Circuit cache is critical because claims are stale after refresh with token rotation
        var circuitRefreshToken = _circuitTokenCache.RefreshToken;
        var claimsRefreshToken = httpContext.User?.FindFirst("refresh_token")?.Value;

        var currentRefreshToken = !string.IsNullOrEmpty(circuitRefreshToken)
            ? circuitRefreshToken
            : claimsRefreshToken;

        if (string.IsNullOrEmpty(currentRefreshToken))
        {
            _logger.LogDebug("No refresh token available");
            return null;
        }

        // FAIL-FAST: Check if this refresh token already failed recently
        // This prevents endless retry loops when the token is invalid
        if (_failedRefreshToken == currentRefreshToken &&
            DateTime.UtcNow - _failedRefreshTime < FailedTokenCacheDuration)
        {
            _logger.LogDebug("Skipping refresh - token already failed recently");
            return null;
        }

        // FAST PATH: Check cache BEFORE acquiring lock
        // Only use cache if it was created for the SAME refresh token (same session)
        // This prevents stale cache from previous login sessions
        if (_lastRefreshedToken is not null &&
            _cachedForRefreshToken == currentRefreshToken &&
            DateTime.UtcNow - _lastRefreshTime < RefreshCacheDuration)
        {
            return _lastRefreshedToken;
        }

        // Prevent concurrent refresh attempts
        if (!await RefreshLock.WaitAsync(TimeSpan.FromSeconds(10), cancellationToken))
        {
            _logger.LogWarning("Token refresh lock acquisition timed out");
            return null;
        }

        try
        {
            // SLOW PATH: Re-check cache after acquiring lock
            // Another caller might have completed refresh while we were waiting
            if (_lastRefreshedToken is not null &&
                _cachedForRefreshToken == currentRefreshToken &&
                DateTime.UtcNow - _lastRefreshTime < RefreshCacheDuration)
            {
                return _lastRefreshedToken;
            }

            var user = httpContext.User;
            if (user?.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            // Get tokens - prefer circuit cache over claims (claims are stale in Blazor circuits)
            var currentAccessToken = !string.IsNullOrEmpty(_circuitTokenCache.AccessToken)
                ? _circuitTokenCache.AccessToken
                : user.FindFirst("access_token")?.Value;

            var refreshToken = !string.IsNullOrEmpty(_circuitTokenCache.RefreshToken)
                ? _circuitTokenCache.RefreshToken
                : user.FindFirst("refresh_token")?.Value;

            var tenant = user.FindFirst("tenant")?.Value ?? "root";

            if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(currentAccessToken))
            {
                return null;
            }

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

            // CRITICAL: Update circuit-scoped cache FIRST, before SignInAsync
            // SignInAsync will fail in Blazor Server SignalR context ("Headers are read-only")
            // but the circuit cache will allow subsequent requests to use the new token
            _circuitTokenCache.UpdateTokens(refreshResponse.Token, refreshResponse.RefreshToken);

            // Cache the refreshed token to prevent race conditions across circuits
            // Store the OLD refresh token (before rotation) so concurrent callers with the same token can use cache
            // Intentionally updating static fields from instance method - coordinating across scoped instances
#pragma warning disable S2696 // Instance members should not write to static fields
            _lastRefreshedToken = refreshResponse.Token;
            _cachedForRefreshToken = refreshToken; // The refresh token we used (before rotation)
            _lastRefreshTime = DateTime.UtcNow;
#pragma warning restore S2696

            _logger.LogInformation("Access token refreshed successfully");

            // Try to update the cookie for future page loads
            // This will fail in Blazor Server SignalR context, which is expected
            try
            {
                var identity = new ClaimsIdentity(newClaims, "Cookies");
                var principal = new ClaimsPrincipal(identity);

                await httpContext.SignInAsync("Cookies", principal, new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                });
            }
            catch (InvalidOperationException)
            {
                // Expected in Blazor Server SignalR context - response has already started
                // The circuit cache has the new tokens, so subsequent requests will work
            }

            return refreshResponse.Token;
        }
        catch (ApiException ex) when (ex.StatusCode == 401)
        {
            // Clear circuit cache
            _circuitTokenCache.Clear();

            // Clear static cache and mark this refresh token as failed to prevent retry loops
#pragma warning disable S2696 // Instance members should not write to static fields
            _lastRefreshedToken = null;
            _cachedForRefreshToken = null;
            _lastRefreshTime = DateTime.MinValue;
            _failedRefreshToken = currentRefreshToken; // Mark as failed
            _failedRefreshTime = DateTime.UtcNow;
#pragma warning restore S2696
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
            RefreshLock.Release();
        }
    }

    public void Dispose()
    {
        // Static semaphore should not be disposed by individual instances
        // It's shared across all scoped instances for the app lifetime
    }
}
