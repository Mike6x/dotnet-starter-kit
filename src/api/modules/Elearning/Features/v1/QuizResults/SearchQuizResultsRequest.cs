using Ardalis.Specification;
using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.Elearning.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Starter.WebApi.Elearning.Features.v1.QuizResults;
public class SearchQuizResultsRequest : PaginationFilter, IRequest<PagedList<QuizResultDto>>
{
    public Guid? QuizId { get; set; }
    public string? UserId { get; set; }
    public bool? IsPass { get; set; }
}

public sealed class SearchQuizResultsHandler(
    [FromKeyedServices("elearning:quizresults")] IReadRepository<QuizResult> repository)
    : IRequestHandler<SearchQuizResultsRequest, PagedList<QuizResultDto>>
{
    public async Task<PagedList<QuizResultDto>> Handle(SearchQuizResultsRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new SearchQuizResultsSpecs(request);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        return new PagedList<QuizResultDto>(items, request!.PageNumber, request!.PageSize, totalCount);
    }
}

public sealed class SearchQuizResultsSpecs : EntitiesByPaginationFilterSpec<QuizResult, QuizResultDto>
{
    public SearchQuizResultsSpecs(SearchQuizResultsRequest request)
        : base(request) =>
        Query
            .Include(e => e.Quiz)
                .Where(e => e.QuizId.Equals(request.QuizId!.Value), request.QuizId.HasValue)
                .Where(e => e.SId != null && e.SId.Equals(request.UserId), !string.IsNullOrEmpty(request.UserId))
                .Where(e => e.IsPass.Equals(request.IsPass!), request.IsPass.HasValue)
                    .OrderByDescending(e => e.EndTime, !request.HasOrderBy());
}
