using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Identity.Contracts.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Identity.Features.v1.Users.DeleteUser;

public static class DeleteUserEndpoint
{
    internal static RouteHandlerBuilder MapDeleteUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapDelete("/{id:guid}", (string id, IUserService service) =>
        {
            return service.DeleteAsync(id);
        })
        .WithName(nameof(DeleteUserEndpoint))
        .WithSummary("delete user profile")
        .RequirePermission("Permissions.Users.Delete")
        .WithDescription("delete user profile");
    }
}