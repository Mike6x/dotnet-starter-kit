using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Identity.Contracts.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Identity.Features.v1.Users.GetUserRoles;

public static class GetUserRolesEndpoint
{
    internal static RouteHandlerBuilder MapGetUserRolesEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/users/{id:guid}/roles", (string id, IUserService service, CancellationToken cancellationToken) =>
        {
            return service.GetUserRolesAsync(id, cancellationToken);
        })
        .WithName("GetUserRoles")
        .WithSummary("Get user roles")
        .RequirePermission("Permissions.Users.View")
        .WithDescription("Retrieve the roles assigned to a specific user.");
    }
}
