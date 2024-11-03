using FluentValidation;
using FSH.Framework.Core.Identity.Users.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Framework.Infrastructure.Identity.Users.Endpoints
{

    public static class GetConfirmPhoneNumberEndPoint
    {
        internal static RouteHandlerBuilder MapGetCornfirmPhoneNumberEndpoint(this IEndpointRouteBuilder endpoints)
        {
            return endpoints.MapPost("/confirm-phone-number", (
                [FromQuery] string userId,
                [FromQuery] string code,
                IUserService userService,
                CancellationToken cancellationToken) =>
            {

                return Task.FromResult(userService.ConfirmPhoneNumberAsync(userId, code));
            })
            .WithName(nameof(GetConfirmPhoneNumberEndPoint))
            .WithSummary("Confirm phone number")
            .WithDescription("Confirm phone number for a user.")
            .AllowAnonymous();
        }
        
    }
}