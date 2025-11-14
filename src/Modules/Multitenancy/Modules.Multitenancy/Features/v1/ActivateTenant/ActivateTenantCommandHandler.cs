using FSH.Modules.Multitenancy.Contracts;
using FSH.Modules.Multitenancy.Contracts.v1.ActivateTenant;
using Mediator;

namespace FSH.Modules.Multitenancy.Features.v1.ActivateTenant;

public sealed class ActivateTenantCommandHandler(ITenantService tenantService)
    : ICommandHandler<ActivateTenantCommand, ActivateTenantCommandResponse>
{
    public async ValueTask<ActivateTenantCommandResponse> Handle(ActivateTenantCommand command, CancellationToken cancellationToken)
    {
        var result = await tenantService.ActivateAsync(command.TenantId, cancellationToken);
        return new ActivateTenantCommandResponse(result, command.TenantId);
    }
}