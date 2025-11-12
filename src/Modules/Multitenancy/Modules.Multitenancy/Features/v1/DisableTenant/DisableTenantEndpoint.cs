using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Multitenancy.Contracts.v1.DisableTenant;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Multitenancy.Features.v1.DisableTenant;

public static class DisableTenantEndpoint
{
    internal static RouteHandlerBuilder Map(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/{id}/deactivate", (IMediator mediator, string id)
            => mediator.Send(new DisableTenantCommand(id)))
                                .WithName("DeactivateTenant")
                                .WithSummary("Deactivate tenant")
                                .RequirePermission("Permissions.Tenants.Update")
                                .WithDescription("Deactivate a tenant.");
    }
}
