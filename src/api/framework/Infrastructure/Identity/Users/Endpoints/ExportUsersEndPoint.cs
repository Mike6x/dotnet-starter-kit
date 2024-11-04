using FSH.Framework.Core.Identity.Users.Abstractions;
using FSH.Framework.Core.Identity.Users.Features.ExportUsers;
using FSH.Framework.Infrastructure.Auth.Policy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Framework.Infrastructure.Identity.Users.Endpoints
{
    public static class ExportUsersEndpoint
    {
        internal static RouteHandlerBuilder MapExportUsersEndpoint(this IEndpointRouteBuilder endpoints)
        {
            return endpoints.MapPost("/export", (UserListFilter filter, IUserService service, CancellationToken cancellationToken) =>
            {
                return service.ExportAsync(filter, cancellationToken);
            })
            .WithName(nameof(ExportUsersEndpoint))
            .WithSummary("Export a list of users with paging support")
            .RequirePermission("Permissions.Users.Export")
            .WithDescription("Export a list of users with paging support");
        }
    }
}