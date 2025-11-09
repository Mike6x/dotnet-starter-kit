using FSH.Modules.Multitenancy.Contracts.Dtos;
using FSH.Modules.Multitenancy.Contracts.v1.GetTenants;
using FSH.Modules.Multitenancy.Services;
using Mapster;
using Mediator;

namespace FSH.Modules.Multitenancy.Features.v1.GetTenants;

public sealed class GetTenantsQueryHandler(ITenantService service)
    : IQueryHandler<GetTenantsQuery, IReadOnlyCollection<TenantDto>>
{
    public async ValueTask<IReadOnlyCollection<TenantDto>> Handle(GetTenantsQuery query, CancellationToken cancellationToken)
    {
        var tenants = await service.GetAllAsync();
        return tenants.Adapt<List<TenantDto>>();
    }
}