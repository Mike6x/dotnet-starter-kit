using FSH.Modules.Multitenancy.Contracts.Dtos;
using FSH.Modules.Multitenancy.Contracts.v1.GetTenantById;
using FSH.Modules.Multitenancy.Services;
using Mapster;
using Mediator;

namespace FSH.Framework.Tenant.Features.v1.GetTenantById;

public sealed class GetTenantByIdQueryHandler(ITenantService service)
    : IQueryHandler<GetTenantByIdQuery, TenantDto>
{
    public async ValueTask<TenantDto> Handle(GetTenantByIdQuery query, CancellationToken cancellationToken)
    {
        var tenant = await service.GetByIdAsync(query.TenantId);
        return tenant.Adapt<TenantDto>();
    }
}