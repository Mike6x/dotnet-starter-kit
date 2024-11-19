using FSH.Framework.Core.Identity.Users.Abstractions;
using FSH.Framework.Infrastructure.Auth.Policy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Framework.Infrastructure.Identity.Users.Endpoints;
public static class DisableUserEndpoint
{
    internal static RouteHandlerBuilder MapDisableUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapDelete("/disable/{id:guid}", (string id, IUserService service) =>
        {
            return service.DisableAsync(id);
        })
        .WithName(nameof(DisableUserEndpoint))
        .WithSummary("Disable user profile")
        .RequirePermission("Permissions.Users.Delete")
        .WithDescription("disable user profile");
    }
}
