using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Identity.Contracts.Services;
using FSH.Modules.Identity.Contracts.v1.Roles.UpsertRole;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Identity.Features.v1.Roles.UpsertRole;

public static class CreateOrUpdateRoleEndpoint
{
    public static RouteHandlerBuilder MapCreateOrUpdateRoleEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/", async ([FromBody] UpsertRoleCommand request, IRoleService roleService) =>
        {
            return await roleService.CreateOrUpdateRoleAsync(request.Id, request.Name, request.Description);
        })
        .WithName(nameof(CreateOrUpdateRoleEndpoint))
        .WithSummary("Create or update a role")
        .RequirePermission("Permissions.Roles.Create")
        .WithDescription("Create a new role or update an existing role.");
    }
}