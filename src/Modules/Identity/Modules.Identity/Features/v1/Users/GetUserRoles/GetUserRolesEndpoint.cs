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
        return endpoints.MapGet("/{id:guid}/roles", (string id, IUserService service) =>
        {
            return service.GetUserRolesAsync(id, CancellationToken.None);
        })
        .WithName(nameof(GetUserRolesEndpoint))
        .WithSummary("get user roles")
        .RequirePermission("Permissions.Users.View")
        .WithDescription("get user roles");
    }
}