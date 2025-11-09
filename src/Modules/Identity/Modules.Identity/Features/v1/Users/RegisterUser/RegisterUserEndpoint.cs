using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Identity.Contracts.Services;
using FSH.Modules.Identity.Contracts.v1.Users.RegisterUser;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Identity.Features.v1.Users.RegisterUser;

public static class RegisterUserEndpoint
{
    internal static RouteHandlerBuilder MapRegisterUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/register", (RegisterUserCommand request,
            IUserService service,
            HttpContext context,
            CancellationToken cancellationToken) =>
        {
            var origin = $"{context.Request.Scheme}://{context.Request.Host.Value}{context.Request.PathBase.Value}";
            return service.RegisterAsync(request.FirstName,
                request.LastName,
                request.Email,
                request.UserName,
                request.Password,
                request.ConfirmPassword,
                request.PhoneNumber,
                origin,
                cancellationToken);
        })
        .WithName(nameof(RegisterUserEndpoint))
        .WithSummary("register user")
        .RequirePermission("Permissions.Users.Create")
        .WithDescription("register user");
    }
}