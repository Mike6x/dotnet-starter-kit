using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Multitenancy.Contracts.v1.GetTenantById;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Multitenancy.Features.v1.GetTenantById;

public static class GetTenantByIdEndpoint
{
    internal static RouteHandlerBuilder Map(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/{id}", (IMediator mediator, string id)
            => mediator.Send(new GetTenantByIdQuery(id)))
                                .WithName("GetTenant")
                                .WithSummary("Get tenant")
                                .RequirePermission("Permissions.Tenants.View")
                                .WithDescription("Retrieve tenant details by unique identifier.");
    }
}
