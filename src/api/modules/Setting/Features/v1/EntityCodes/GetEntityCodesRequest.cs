using FSH.Framework.Core.Paging;
using FSH.Starter.WebApi.Setting.Domain;
using MediatR;
using FSH.Framework.Core.Persistence;
using Microsoft.Extensions.DependencyInjection;
using FSH.Framework.Core.Specifications;
using Ardalis.Specification;

namespace FSH.Starter.WebApi.Setting.Features.v1.EntityCodes;


public class GetEntityCodesRequest : BaseFilter, IRequest<List<EntityCodeDto>>
{
    public CodeType? Type { get; set; }
}


public class GetEntityCodesHandler(
    [FromKeyedServices("setting:EntityCode")] IReadRepository<EntityCode> repository)
    : IRequestHandler<GetEntityCodesRequest, List<EntityCodeDto>>
{
    public async Task<List<EntityCodeDto>> Handle(GetEntityCodesRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new GetEntityCodesSpecs( request);

        return await repository.ListAsync(spec, cancellationToken);
    }
}

public sealed class GetEntityCodesSpecs: EntitiesByBaseFilterSpec<EntityCode, EntityCodeDto>
{
    public GetEntityCodesSpecs(GetEntityCodesRequest request)
        : base(request) =>
        Query
            .Where(e => e.Type.Equals(request.Type!.Value), request.Type.HasValue)
            .OrderBy(e => e.Order);
}
