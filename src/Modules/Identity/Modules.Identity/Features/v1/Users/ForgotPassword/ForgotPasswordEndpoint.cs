using FluentValidation;
using FluentValidation.Results;
using FSH.Framework.Shared.Multitenancy;
using FSH.Framework.Web.Origin;
using FSH.Modules.Identity.Contracts.Services;
using FSH.Modules.Identity.Contracts.v1.Users.ForgotPassword;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace FSH.Modules.Identity.Features.v1.Users.ForgotPassword;

public static class ForgotPasswordEndpoint
{
    internal static RouteHandlerBuilder MapForgotPasswordEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/forgot-password", async (HttpRequest request,
            [FromHeader(Name = MultitenancyConstants.Identifier)] string tenant,
            [FromBody] ForgotPasswordCommand command,
            IOptions<OriginOptions> settings,
            IValidator<ForgotPasswordCommand> validator,
            IUserService userService,
            CancellationToken cancellationToken) =>
        {
            ValidationResult result = await validator.ValidateAsync(command, cancellationToken);
            if (!result.IsValid)
            {
                return Results.ValidationProblem(result.ToDictionary());
            }

            // Obtain origin from appsettings
            var origin = settings.Value;

            if (origin?.OriginUrl == null)
            {
                // Handle the case where OriginUrl is null
                return Results.BadRequest("Origin URL is not configured.");
            }

            await userService.ForgotPasswordAsync(command.Email, origin.OriginUrl.ToString(), cancellationToken);
            return Results.Ok("Password reset email sent.");
        })
        .WithName(nameof(ForgotPasswordEndpoint))
        .WithSummary("Forgot password")
        .WithDescription("Generates a password reset token and sends it via email.")
        .AllowAnonymous();
    }

}