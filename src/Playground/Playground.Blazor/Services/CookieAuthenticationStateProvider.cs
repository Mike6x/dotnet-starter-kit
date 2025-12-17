using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;

namespace FSH.Playground.Blazor.Services;

/// <summary>
/// Simple authentication state provider that reads from cookie authentication.
/// Uses the built-in ServerAuthenticationStateProvider which automatically reads HttpContext.User.
/// </summary>
public sealed class CookieAuthenticationStateProvider : ServerAuthenticationStateProvider
{
    // This class intentionally has no custom logic.
    // ServerAuthenticationStateProvider automatically reads from HttpContext.User,
    // which is populated by the ASP.NET Core Cookie Authentication middleware.
}
