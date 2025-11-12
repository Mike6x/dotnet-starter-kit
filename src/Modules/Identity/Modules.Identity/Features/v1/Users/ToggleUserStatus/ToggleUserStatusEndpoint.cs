using FluentValidation;
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
        return endpoints.MapPost("/{id:guid}/toggle-status", async (
            [FromQuery] string id,
            [FromBody] ToggleUserStatusCommand command,
            [FromServices] IUserService userService,
            CancellationToken cancellationToken) =>
        {
            if (id != command.UserId)
            {
                return Results.BadRequest();
            }

            await userService.ToggleStatusAsync(command.ActivateUser, command.UserId, cancellationToken);
            return Results.Ok();
        })
        .WithName("ToggleUserStatus")
        .WithSummary("Toggle user status")
        .WithDescription("Activate or deactivate a user account.")
        .AllowAnonymous();
    }

}
