using FSH.Framework.Core.Identity.Users.Abstractions;
using FSH.Framework.Infrastructure.Auth.Policy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Framework.Infrastructure.Identity.Users.Endpoints;
public static class GetUserByEmailEndpoint
{
    internal static RouteHandlerBuilder MapGetUserByEmailEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/email/{email}", (string email, IUserService service) =>
        {
            return service.GetByEmailAsync(email, CancellationToken.None);
        })
        .WithName(nameof(GetUserByEmailEndpoint))
        .WithSummary("Get user profile by Name")
        .RequirePermission("Permissions.Users.View")
        .WithDescription("Get another user's profile details by userName.");
    }
}
