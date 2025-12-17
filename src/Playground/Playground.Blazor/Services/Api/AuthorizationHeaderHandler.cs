using Microsoft.AspNetCore.Components.Authorization;

namespace FSH.Playground.Blazor.Services.Api;

/// <summary>
/// Delegating handler that adds the JWT token to API requests
/// </summary>
internal sealed class AuthorizationHeaderHandler : DelegatingHandler
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly ILogger<AuthorizationHeaderHandler> _logger;

    public AuthorizationHeaderHandler(
        AuthenticationStateProvider authenticationStateProvider,
        ILogger<AuthorizationHeaderHandler> logger)
    {
        _authenticationStateProvider = authenticationStateProvider;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        try
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user?.Identity?.IsAuthenticated == true)
            {
                // Get the JWT token from claims (stored during login)
                var token = user.FindFirst("access_token")?.Value;

                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to attach authorization header");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
