using FSH.Framework.Core.Exceptions;
using FSH.Framework.Shared.Identity.Authorization;
using FSH.Framework.Shared.Identity.Claims;
using FSH.Modules.Identity.Contracts.Services;
using FSH.Modules.Identity.Contracts.v1.Users.UpdateUser;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace FSH.Modules.Identity.Features.v1.Users.UpdateUser;

public static class UpdateUserEndpoint
{
    internal static RouteHandlerBuilder MapUpdateUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPut("/profile", ([FromBody] UpdateUserCommand request, ClaimsPrincipal user, IUserService service) =>
        {
            if (user.GetUserId() is not { } userId || string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedException();
            }
            return service.UpdateAsync(request.Id,
                request.FirstName,
                request.LastName,
                request.PhoneNumber,
                request.Image,
                request.DeleteCurrentImage);
        })
        .WithName("UpdateUserProfile")
        .WithSummary("Update user profile")
        .RequirePermission("Permissions.Users.Update")
        .WithDescription("Update profile details for the authenticated user.");
    }
}
