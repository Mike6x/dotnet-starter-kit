using FluentValidation;
using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Identity.Contracts.Services;
using FSH.Modules.Identity.Contracts.v1.Users.ToggleUserStatus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Identity.Features.v1.Users.ToggleUserStatus;

public static class ToggleUserStatusEndpoint
{
    internal static RouteHandlerBuilder ToggleUserStatusEndpointEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPatch("/users/{id:guid}", async (
            string id,
            [FromBody] ToggleUserStatusCommand command,
            [FromServices] IUserService userService,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(command.UserId))
            {
                command.UserId = id;
            }

            if (!string.Equals(id, command.UserId, StringComparison.Ordinal))
            {
                return Results.BadRequest();
            }

            await userService.ToggleStatusAsync(command.ActivateUser, command.UserId, cancellationToken);
            return Results.NoContent();
        })
        .WithName("ToggleUserStatus")
        .WithSummary("Toggle user status")
        .RequirePermission("Permissions.Users.Update")
        .WithDescription("Activate or deactivate a user account.");
    }

}
