using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Identity.Contracts.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Identity.Features.v1.Roles.GetRole;

public static class GetRoleByIdEndpoint
{
    public static RouteHandlerBuilder MapGetRoleByIdEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/roles/{id:guid}", async (string id, IRoleService roleService) =>
        {
            return await roleService.GetRoleAsync(id);
        })
        .WithName(nameof(GetRoleByIdEndpoint))
        .WithSummary("Get role details by ID")
        .RequirePermission("Permissions.Roles.View")
        .WithDescription("Retrieve the details of a role by its ID.");
    }
}