using FSH.Modules.Multitenancy.Contracts.Dtos;
using Mediator;

namespace FSH.Modules.Multitenancy.Contracts.v1.GetTenantById;

public sealed record GetTenantByIdQuery(string TenantId) : IQuery<TenantDto>;