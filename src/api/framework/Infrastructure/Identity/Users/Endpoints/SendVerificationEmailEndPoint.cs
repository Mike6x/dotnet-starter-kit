using FSH.Framework.Core.Identity.Users.Abstractions;
using FSH.Framework.Infrastructure.Auth.Policy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Framework.Infrastructure.Identity.Users.Endpoints
{
    public static class SendVerificationEmailEndPoint
    {
        internal static RouteHandlerBuilder MapSendVerificationEmailEndPoint(this IEndpointRouteBuilder endpoints)
        {
            return endpoints.MapPost("/{id:guid}/verification-email", async (
                string id,
                HttpContext context,
                [FromServices] IUserService userService,
                CancellationToken cancellationToken) =>
                {
                    var origin = $"{context.Request.Scheme}://{context.Request.Host.Value}{context.Request.PathBase.Value}";

                    await userService.SendVerificationEmailAsync(id, origin, cancellationToken);
                    return Results.Ok();
                })
                .WithName(nameof(SendVerificationEmailEndPoint))
                .WithSummary("Send email to verify user")
                .RequirePermission("Permissions.Users.Update")
                .WithDescription("Send email to verify user");
        }
    }
}