using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Identity.Contracts.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Identity.Features.v1.Users.GetUsers;

public static class GetUsersListEndpoint
{
    internal static RouteHandlerBuilder MapGetUsersListEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/users", (CancellationToken cancellationToken, IUserService service) =>
        {
            return service.GetListAsync(cancellationToken);
        })
        .WithName("ListUsers")
        .WithSummary("List users")
        .RequirePermission("Permissions.Users.View")
        .WithDescription("Retrieve a list of users for the current tenant.");
    }
}
