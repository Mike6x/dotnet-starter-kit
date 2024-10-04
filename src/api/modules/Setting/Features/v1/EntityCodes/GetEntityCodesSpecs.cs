using Ardalis.Specification;
using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.Setting.Domain;

namespace FSH.Starter.WebApi.Setting.Features.v1.EntityCodes;

public sealed class GetEntityCodesSpecs: EntitiesByBaseFilterSpec<EntityCode, EntityCodeDto>
{
    public GetEntityCodesSpecs(GetEntityCodesRequest request)
        : base(request) =>
        Query
            .Where(e => e.Type.Equals(request.Type!.Value), request.Type.HasValue)
            .OrderBy(e => e.Order);
}
