using FluentValidation;
using FluentValidation.Results;
using FSH.Framework.Shared.Identity.Claims;
using FSH.Framework.Web.Origin;
using FSH.Modules.Identity.Contracts.Services;
using FSH.Modules.Identity.Contracts.v1.Users.ChangePassword;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace FSH.Modules.Identity.Features.v1.Users.ChangePassword;

public static class ChangePasswordEndpoint
{
    internal static RouteHandlerBuilder MapChangePasswordEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/change-password", async (ChangePasswordCommand command,
            HttpContext context,
            IOptions<OriginOptions> settings,
            [FromServices] IValidator<ChangePasswordCommand> validator,
             IUserService userService,
            CancellationToken cancellationToken) =>
        {
            ValidationResult result = await validator.ValidateAsync(command, cancellationToken);
            if (!result.IsValid)
            {
                return Results.ValidationProblem(result.ToDictionary());
            }

            if (context.User.GetUserId() is not { } userId || string.IsNullOrEmpty(userId))
            {
                return Results.BadRequest();
            }

            await userService.ChangePasswordAsync(command.Password, command.NewPassword, command.ConfirmNewPassword, userId);
            return Results.Ok("password reset email sent");
        })
        .WithName(nameof(ChangePasswordEndpoint))
        .WithSummary("Changes password")
        .WithDescription("Change password");
    }

}