using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Identity.Contracts.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Identity.Features.v1.Roles.GetRoles;

public static class GetRolesEndpoint
{
    public static RouteHandlerBuilder MapGetRolesEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/roles", async (IRoleService roleService) =>
        {
            return await roleService.GetRolesAsync();
        })
        .WithName(nameof(GetRolesEndpoint))
        .WithSummary("Get a list of all roles")
        .RequirePermission("Permissions.Roles.View")
        .WithDescription("Retrieve a list of all roles available in the system.");
    }
}