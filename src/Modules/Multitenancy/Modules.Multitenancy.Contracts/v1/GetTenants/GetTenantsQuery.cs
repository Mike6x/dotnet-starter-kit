using FSH.Modules.Multitenancy.Contracts.Dtos;
using Mediator;

namespace FSH.Modules.Multitenancy.Contracts.v1.GetTenants;

public sealed record GetTenantsQuery : IQuery<IReadOnlyCollection<TenantDto>>;