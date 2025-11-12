using FSH.Modules.Identity.Contracts.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Identity.Features.v1.Users.ConfirmEmail;

public static class ConfirmEmailEndpoint
{
    internal static RouteHandlerBuilder MapConfirmEmailEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/confirm-email", (string userId, string code, string tenant, IUserService service) =>
        {
            return service.ConfirmEmailAsync(userId, code, tenant, default);
        })
        .WithName("ConfirmEmail")
        .WithSummary("Confirm user email")
        .WithDescription("Confirm a user's email address.")
        .AllowAnonymous();
    }
}
