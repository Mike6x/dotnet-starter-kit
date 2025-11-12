using FluentValidation;
using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Identity.Contracts.Services;
using FSH.Modules.Identity.Contracts.v1.Roles.UpdatePermissions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Identity.Features.v1.Roles.UpdateRolePermissions;

public static class UpdateRolePermissionsEndpoint
{
    public static RouteHandlerBuilder MapUpdateRolePermissionsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPut("/{id}/permissions", async (
            [FromBody] UpdatePermissionsCommand request,
            IRoleService roleService,
            string id,
            [FromServices] IValidator<UpdatePermissionsCommand> validator) =>
        {
            if (id != request.RoleId) return Results.BadRequest();
            var response = await roleService.UpdatePermissionsAsync(request.RoleId, request.Permissions);
            return Results.Ok(response);
        })
        .WithName("UpdateRolePermissions")
        .WithSummary("Update role permissions")
        .RequirePermission("Permissions.Roles.Create")
        .WithDescription("Replace the set of permissions assigned to a role.");
    }
}
