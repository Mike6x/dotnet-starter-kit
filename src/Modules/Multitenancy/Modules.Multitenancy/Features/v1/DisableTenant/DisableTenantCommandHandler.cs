using FSH.Modules.Multitenancy.Contracts;
using FSH.Modules.Multitenancy.Contracts.v1.DisableTenant;
using Mediator;

namespace FSH.Modules.Multitenancy.Features.v1.DisableTenant;

public class DisableTenantCommandHandler(ITenantService service)
    : ICommandHandler<DisableTenantCommand, DisableTenantCommandResponse>
{
    public async ValueTask<DisableTenantCommandResponse> Handle(DisableTenantCommand command, CancellationToken cancellationToken)
    {
        var status = await service.DeactivateAsync(command.TenantId);
        return new DisableTenantCommandResponse(status);
    }
}