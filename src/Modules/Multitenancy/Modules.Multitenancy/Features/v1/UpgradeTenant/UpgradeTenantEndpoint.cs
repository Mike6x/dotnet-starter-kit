using FSH.Framework.Shared.Identity.Authorization;
using FSH.Modules.Multitenancy.Contracts.v1.UpgradeTenant;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Modules.Multitenancy.Features.v1.UpgradeTenant;

public static class UpgradeTenantEndpoint
{
    internal static RouteHandlerBuilder Map(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/upgrade", ([FromBody] UpgradeTenantCommand command, IMediator dispatcher)
            => dispatcher.Send(command))
                            .WithName(nameof(UpgradeTenantEndpoint))
                            .WithSummary("upgrade tenant subscription")
                            .RequirePermission("Permissions.Tenants.Update")
                            .WithDescription("upgrade tenant subscription");
    }
}