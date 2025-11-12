using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Identity.Contracts.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Identity.Features.v1.Roles.DeleteRole;

public static class DeleteRoleEndpoint
{
    public static RouteHandlerBuilder MapDeleteRoleEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapDelete("/{id:guid}", async (string id, IRoleService roleService) =>
        {
            await roleService.DeleteRoleAsync(id);
        })
        .WithName("DeleteRole")
        .WithSummary("Delete role by ID")
        .RequirePermission("Permissions.Roles.Delete")
        .WithDescription("Remove an existing role by its unique identifier.");
    }
}
