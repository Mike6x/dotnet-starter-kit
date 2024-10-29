using FSH.Framework.Core.Paging;
using MediatR;
using Ardalis.Specification;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.Setting.Domain;
using FSH.Framework.Core.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Starter.WebApi.Setting.Features.v1.Dimensions;
public class SearchDimensionsRequest : PaginationFilter, IRequest<PagedList<DimensionDto>>
{
    public string? Type { get;  set; }
    public Guid? FatherId { get; set; }
    public bool? IsActive { get; set; }
}


public sealed class SearchDimensionsHandler(
    [FromKeyedServices("setting:dimension")] IReadRepository<Dimension> repository)
    : IRequestHandler<SearchDimensionsRequest, PagedList<DimensionDto>>
{
    public async Task<PagedList<DimensionDto>> Handle(SearchDimensionsRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new SearchDimensionsSpecs(request);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        return new PagedList<DimensionDto>(items, request!.PageNumber, request!.PageSize, totalCount);
    }
}
public sealed class SearchDimensionsSpecs : EntitiesByPaginationFilterSpec<Dimension, DimensionDto>
{
    public SearchDimensionsSpecs(SearchDimensionsRequest request)
        : base(request) =>
        Query
            .Where(e => e.IsActive.Equals(request.IsActive!), request.IsActive.HasValue)
            .Where(e => e.Type.Equals(request.Type), !string.IsNullOrEmpty(request.Type))
            .Where(e => e.FatherId.Equals(request.FatherId!.Value), request.FatherId.HasValue)
            .OrderBy(e => e.Order, !request.HasOrderBy());
}
