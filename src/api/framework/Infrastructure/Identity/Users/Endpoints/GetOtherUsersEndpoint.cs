using FSH.Framework.Core.Identity.Users.Abstractions;
using FSH.Framework.Infrastructure.Auth.Policy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Framework.Infrastructure.Identity.Users.Endpoints
{
    public static class  GetOtherUsersEndpoint
    {
        internal static RouteHandlerBuilder MapGetOtherUsersEndpoint(this IEndpointRouteBuilder endpoints)
        {
            return endpoints.MapGet("{userId}/otherusers", async (string userId,IUserService service, CancellationToken cancellationToken) =>
            {
                var list = await service.GetListAsync(cancellationToken);

                return list.Where(user => user.Id.ToString() != userId).ToList();
            })
            .WithName(nameof(GetOtherUsersEndpoint))
            .WithSummary("get others")
            .RequirePermission("Permissions.Users.Search")
            .WithDescription("Get list of other users");
        }
        
    }
}