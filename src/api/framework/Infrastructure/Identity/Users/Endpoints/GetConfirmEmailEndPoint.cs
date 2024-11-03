using FluentValidation;
using FSH.Framework.Core.Identity.Users.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Framework.Infrastructure.Identity.Users.Endpoints
{
    public static class GetConirmEmailEndPoint
    {
        internal static RouteHandlerBuilder MapGetCornfirmEmailEndpoint(this IEndpointRouteBuilder endpoints)
        {
            return endpoints.MapPost("/confirm-email", (
                [FromQuery] string tenant,
                [FromQuery] string userId,
                [FromQuery] string code,
                IUserService userService,
                CancellationToken cancellationToken) =>
            {

                return Task.FromResult(userService.ConfirmEmailAsync(userId, code, tenant, cancellationToken));
            })
            .WithName(nameof(GetConirmEmailEndPoint))
            .WithSummary("Confirm email")
            .WithDescription("Confirm email address for a user.")
            .AllowAnonymous();
        }
        
    }
}