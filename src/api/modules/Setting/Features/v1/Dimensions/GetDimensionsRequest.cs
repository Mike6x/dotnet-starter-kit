using Ardalis.Specification;
using FSH.Framework.Core.Paging;
using MediatR;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.Setting.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Starter.WebApi.Setting.Features.v1.Dimensions;

public class GetDimensionsRequest : BaseFilter, IRequest<List<DimensionDto>>
{
    public string? Type { get;  set; }
    public Guid? FatherId { get; set; }
    public bool? IsActive { get; set; }
}

public class GetDimensionsHandler(
    [FromKeyedServices("setting:dimension")] IReadRepository<Dimension> repository)
    : IRequestHandler<GetDimensionsRequest, List<DimensionDto>>
{
    public async Task<List<DimensionDto>> Handle(GetDimensionsRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        var spec = new GetDimensionsSpecs(request);
        
        return await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
    }
}

public sealed class GetDimensionsSpecs : EntitiesByBaseFilterSpec<Dimension, DimensionDto>
{
    public GetDimensionsSpecs(GetDimensionsRequest request)
        : base(request) =>
        Query
            .Where(e => e.IsActive.Equals(request.IsActive!), request.IsActive.HasValue)
            .Where(e => string.IsNullOrEmpty(request.Type) || e.Type.Equals(request.Type, StringComparison.Ordinal))
            .Where(e => e.FatherId.Equals(request.FatherId!.Value), request.FatherId.HasValue)
            .OrderBy(e => e.Order);
}
