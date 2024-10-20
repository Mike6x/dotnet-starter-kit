using Ardalis.Specification;
using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.Elearning.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Starter.WebApi.Elearning.Features.v1.QuizResults;

public class GetQuizResultsRequest : BaseFilter, IRequest<List<QuizResultDto>>
{

    public string? QuizId { get; set; }
    public string? UserId { get; set; }
     public bool? IsPass { get; set; }
}

public class GetQuizResultsHandler(
    [FromKeyedServices("elearning:quizresults")] IReadRepository<QuizResult> repository)
    : IRequestHandler<GetQuizResultsRequest, List<QuizResultDto>>
{
    public async Task<List<QuizResultDto>> Handle(GetQuizResultsRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        var spec = new GetQuizResultsSpecs(request);
        
        return await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
    }
}

public sealed class GetQuizResultsSpecs : EntitiesByBaseFilterSpec<QuizResult, QuizResultDto>
{
    public GetQuizResultsSpecs(GetQuizResultsRequest request)
        : base(request) =>
        Query
            .Where(e => e.QuizId.ToString().Equals(request.QuizId, StringComparison.Ordinal), !string.IsNullOrEmpty(request.QuizId))
            .Where(e => e.SId != null && e.SId.Equals(request.UserId), !string.IsNullOrEmpty(request.UserId))
            .Where(e => e.IsPass.Equals(request.IsPass!), request.IsPass.HasValue)
                .OrderBy(e => e.EndTime);
}
