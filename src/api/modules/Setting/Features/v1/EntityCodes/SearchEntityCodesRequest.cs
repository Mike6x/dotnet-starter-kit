using FSH.Framework.Core.Paging;
using FSH.Starter.WebApi.Setting.Domain;
using MediatR;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Specifications;
using Microsoft.Extensions.DependencyInjection;
using Ardalis.Specification;

namespace FSH.Starter.WebApi.Setting.Features.v1.EntityCodes;

public class SearchEntityCodesRequest : PaginationFilter, IRequest<PagedList<EntityCodeDto>>
{
    public CodeType? Type { get; set; }
}


public sealed class SearchEntityCodesHandler(
    [FromKeyedServices("setting:EntityCode")] IReadRepository<EntityCode> repository)
    : IRequestHandler<SearchEntityCodesRequest, PagedList<EntityCodeDto>>
{
    public async Task<PagedList<EntityCodeDto>> Handle(SearchEntityCodesRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new SearchEntityCodesSpecs(request);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        return new PagedList<EntityCodeDto>(items, request.PageNumber, request.PageSize, totalCount);
    }
}

public sealed class SearchEntityCodesSpecs: EntitiesByPaginationFilterSpec<EntityCode, EntityCodeDto>
{
    public SearchEntityCodesSpecs(SearchEntityCodesRequest request)
        : base(request) =>
        Query
            .Where(e => e.Type.Equals(request.Type!.Value), request.Type.HasValue)
            .OrderBy(e => e.Order, !request.HasOrderBy());
}

