using Mediator;

namespace FSH.Modules.Multitenancy.Contracts.v1.ActivateTenant;

public sealed record ActivateTenantCommand(string TenantId) : ICommand<ActivateTenantCommandResponse>;