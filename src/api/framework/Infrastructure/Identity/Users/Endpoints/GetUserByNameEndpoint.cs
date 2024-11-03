using FSH.Framework.Core.Identity.Users.Abstractions;
using FSH.Framework.Infrastructure.Auth.Policy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Framework.Infrastructure.Identity.Users.Endpoints;
public static class GetUserByNameEndpoint
{
    internal static RouteHandlerBuilder MapGetUserByNameEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/username/{userName}", (string userName, IUserService service) =>
        {
            return service.GetByNameAsync(userName, CancellationToken.None);
        })
        .WithName(nameof(GetUserByNameEndpoint))
        .WithSummary("Get user profile by Name")
        .RequirePermission("Permissions.Users.View")
        .WithDescription("Get another user's profile details by userName.");
    }
}
