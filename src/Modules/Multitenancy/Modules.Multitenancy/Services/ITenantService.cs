using FSH.Modules.Multitenancy.Contracts.Dtos;

namespace FSH.Modules.Multitenancy.Services;

public interface ITenantService
{
    Task<List<TenantDto>> GetAllAsync();

    Task<bool> ExistsWithIdAsync(string id);

    Task<bool> ExistsWithNameAsync(string name);

    Task<TenantDto> GetByIdAsync(string id);

    Task<string> CreateAsync(string id, string name, string? connectionString, string adminEmail, string? issuer, CancellationToken cancellationToken);

    Task<string> ActivateAsync(string id, CancellationToken cancellationToken);

    Task<string> DeactivateAsync(string id);

    Task<DateTime> UpgradeSubscription(string id, DateTime extendedExpiryDate);
}