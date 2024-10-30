using Ardalis.Specification;
using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.Elearning.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Starter.WebApi.Elearning.Features.v1.Quizs;
public class SearchQuizsRequest : PaginationFilter, IRequest<PagedList<QuizDto>>
{
    public Guid? QuizTypeId { get; set; }
    public Guid? QuizTopicId { get; set; }
    public Guid? QuizModeId { get; set; }

    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    public bool? IsActive { get; set; }
}

public sealed class SearchQuizsHandler(
    [FromKeyedServices("elearning:quizs")] IReadRepository<Quiz> repository)
    : IRequestHandler<SearchQuizsRequest, PagedList<QuizDto>>
{
    public async Task<PagedList<QuizDto>> Handle(SearchQuizsRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new SearchQuizsSpecs(request);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false) ?? [];
        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        return new PagedList<QuizDto>(items, request!.PageNumber, request!.PageSize, totalCount);
    }
}

public sealed class SearchQuizsSpecs : EntitiesByPaginationFilterSpec<Quiz, QuizDto>
{
    public SearchQuizsSpecs(SearchQuizsRequest request)
        : base(request) =>
        Query
            .Where(e => e.IsActive.Equals(request.IsActive!), request.IsActive.HasValue)
            .Where(e => e.QuizTypeId.Equals(request.QuizTypeId!), request.QuizTypeId.HasValue)
            .Where(e => e.QuizTopicId.Equals(request.QuizTopicId!), request.QuizTopicId.HasValue)
            .Where(e => e.QuizModeId.Equals(request.QuizModeId!), request.QuizModeId.HasValue)
            .Where(e => e.FromDate >= request.FromDate, request.FromDate.HasValue)
            .Where(e => e.ToDate <= request.ToDate, request.ToDate.HasValue)
                .OrderBy(e => e.Order, !request.HasOrderBy());
}
