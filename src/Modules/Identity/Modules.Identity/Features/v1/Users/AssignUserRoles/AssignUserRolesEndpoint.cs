using FSH.Modules.Identity.Contracts.v1.Users.AssignUserRoles;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Identity.Features.v1.Users.AssignUserRoles;

public static class AssignUserRolesEndpoint
{
    internal static RouteHandlerBuilder MapAssignUserRolesEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/{id:guid}/roles", async (AssignUserRolesCommand command,
            HttpContext context,
            string id,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("AssignUserRoles")
        .WithSummary("Assign roles to user")
        .WithDescription("Assign one or more roles to a user.");
    }
}
