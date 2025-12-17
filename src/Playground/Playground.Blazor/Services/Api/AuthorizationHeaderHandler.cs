using Microsoft.AspNetCore.Authentication;
using System.Net;

namespace FSH.Playground.Blazor.Services.Api;

/// <summary>
/// Delegating handler that adds the JWT token to API requests and handles 401 responses
/// by attempting to refresh the access token. If refresh fails, signs out the user.
/// </summary>
internal sealed class AuthorizationHeaderHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AuthorizationHeaderHandler> _logger;

    public AuthorizationHeaderHandler(
        IHttpContextAccessor httpContextAccessor,
        IServiceProvider serviceProvider,
        ILogger<AuthorizationHeaderHandler> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Attach current access token
        var accessToken = await GetAccessTokenAsync();
        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        }

        // Send the request
        var response = await base.SendAsync(request, cancellationToken);

        // If we get a 401, try to refresh the token and retry once
        if (response.StatusCode == HttpStatusCode.Unauthorized && !string.IsNullOrEmpty(accessToken))
        {
            _logger.LogInformation("Received 401 response, attempting token refresh");

            var newAccessToken = await TryRefreshTokenAsync(cancellationToken);

            if (!string.IsNullOrEmpty(newAccessToken))
            {
                _logger.LogInformation("Token refresh successful, retrying request");

                // Clone the request with new token
                using var retryRequest = await CloneHttpRequestMessageAsync(request);
                retryRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", newAccessToken);

                // Dispose the original response before retrying
                response.Dispose();

                // Retry the request with the new token
                response = await base.SendAsync(retryRequest, cancellationToken);
            }
            else
            {
                _logger.LogWarning("Token refresh failed, signing out user and returning 401 response");

                // Sign out the user since refresh token is also invalid/expired
                await SignOutUserAsync();
            }
        }

        return response;
    }

    private async Task SignOutUserAsync()
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext is not null)
            {
                await httpContext.SignOutAsync("Cookies");
                _logger.LogInformation("User signed out due to expired refresh token");

                // Redirect to login page with session expired message
                httpContext.Response.Redirect("/login?toast=session_expired");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sign out user after token refresh failure");
        }
    }

    private async Task<string?> GetAccessTokenAsync()
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var user = httpContext?.User;

            if (user?.Identity?.IsAuthenticated == true)
            {
                return user.FindFirst("access_token")?.Value;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get access token from claims");
        }

        return null;
    }

    private async Task<string?> TryRefreshTokenAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Resolve the token refresh service from the service provider
            // We use IServiceProvider to avoid circular dependency issues
            var tokenRefreshService = _serviceProvider.GetService<ITokenRefreshService>();
            if (tokenRefreshService is null)
            {
                _logger.LogWarning("TokenRefreshService is not registered");
                return null;
            }

            return await tokenRefreshService.TryRefreshTokenAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return null;
        }
    }

    private static async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage request)
    {
        var clone = new HttpRequestMessage(request.Method, request.RequestUri)
        {
            Version = request.Version
        };

        // Copy headers (except Authorization which we'll set separately)
        foreach (var header in request.Headers.Where(h => !string.Equals(h.Key, "Authorization", StringComparison.OrdinalIgnoreCase)))
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        // Copy content if present
        if (request.Content != null)
        {
            var contentBytes = await request.Content.ReadAsByteArrayAsync();
            clone.Content = new ByteArrayContent(contentBytes);

            // Copy content headers
            foreach (var header in request.Content.Headers)
            {
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        // Copy options
        foreach (var option in request.Options)
        {
            clone.Options.TryAdd(option.Key, option.Value);
        }

        return clone;
    }
}
