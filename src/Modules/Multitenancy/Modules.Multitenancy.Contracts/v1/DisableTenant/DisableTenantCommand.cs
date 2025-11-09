using Mediator;

namespace FSH.Modules.Multitenancy.Contracts.v1.DisableTenant;

public sealed record DisableTenantCommand(string TenantId) : ICommand<DisableTenantCommandResponse>;