using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Multitenancy.Contracts.v1.ActivateTenant;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Tenant.Features.v1.ActivateTenant;

public static class ActivateTenantEndpoint
{
    public static RouteHandlerBuilder Map(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/{id}/activate", async ([FromServices] IMediator mediator, string id)
            => await mediator.Send(new ActivateTenantCommand(id)))
                                .WithName("ActivateTenant")
                                .WithSummary("Activate tenant")
                                .RequirePermission("Permissions.Tenants.Update")
                                .WithDescription("Activate a tenant.");
    }
}
