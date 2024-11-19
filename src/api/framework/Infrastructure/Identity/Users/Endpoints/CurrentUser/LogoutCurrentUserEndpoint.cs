using System.Security.Claims;
using FSH.Framework.Core.Exceptions;
using FSH.Framework.Core.Identity.Users.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shared.Authorization;

namespace FSH.Framework.Infrastructure.Identity.Users.Endpoints;
public static class LogoutCurrentUserEndpoint
{
    internal static RouteHandlerBuilder MapLogoutCurrentUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/logout", (
            ISender mediator, 
            ClaimsPrincipal user, 
            IUserService service, CancellationToken cancellationToken) =>
        {
            return user.GetUserId() is not { } userId || string.IsNullOrEmpty(userId)
                ? throw new UnauthorizedException()
                : service.ChangeOnlineStatusAsync(userId, false, cancellationToken);
        })
        .WithName(nameof(LogoutCurrentUserEndpoint))
        .WithSummary("update online status")
        .WithDescription("Update profile of currently logged in user.");
    }
}
