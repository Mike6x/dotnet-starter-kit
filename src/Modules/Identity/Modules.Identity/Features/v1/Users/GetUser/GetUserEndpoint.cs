using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Identity.Contracts.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Identity.Features.v1.Users.GetUser;

public static class GetUserEndpoint
{
    internal static RouteHandlerBuilder MapGetUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/users/{id:guid}", (string id, IUserService service, CancellationToken cancellationToken) =>
        {
            return service.GetAsync(id, cancellationToken);
        })
        .WithName("GetUser")
        .WithSummary("Get user by ID")
        .RequirePermission("Permissions.Users.View")
        .WithDescription("Retrieve a user's profile details by unique user identifier.");
    }
}
