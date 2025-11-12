using FSH.Framework.Shared.Identity.Authorization;
using FSH.Framework.Shared.Multitenancy;
using FSH.Modules.Multitenancy.Contracts.v1.GetTenants;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Multitenancy.Features.v1.GetTenants;

public static class GetTenantsEndpoint
{
    public static RouteHandlerBuilder Map(IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/", (IMediator mediator)
            => mediator.Send(new GetTenantsQuery()))
                                .WithName("ListTenants")
                                .WithSummary("List tenants")
                                .RequirePermission(MultitenancyConstants.Permissions.View)
                                .WithDescription("Retrieve all tenants.");
    }
}
