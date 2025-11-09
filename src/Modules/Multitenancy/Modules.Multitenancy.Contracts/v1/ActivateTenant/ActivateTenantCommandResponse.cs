namespace FSH.Modules.Multitenancy.Contracts.v1.ActivateTenant;

public sealed record ActivateTenantCommandResponse(string TenantId, string Status);