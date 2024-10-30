using Ardalis.Specification;
using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.Elearning.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Starter.WebApi.Elearning.Features.v1.Quizs;

public class GetQuizsRequest : BaseFilter, IRequest<List<QuizDto>>
{
    public Guid? QuizTypeId { get; set; }
    public Guid? QuizTopicId { get; set; }
    public Guid? QuizModeId { get; set; }

    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    public bool? IsActive { get; set; }
}

public class GetQuizsHandler(
    [FromKeyedServices("elearning:quizs")] IReadRepository<Quiz> repository)
    : IRequestHandler<GetQuizsRequest, List<QuizDto>>
{
    public async Task<List<QuizDto>> Handle(GetQuizsRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        var spec = new GetQuizsSpecs(request);
        
        return await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
    }
}

public sealed class GetQuizsSpecs : EntitiesByBaseFilterSpec<Quiz, QuizDto>
{
    public GetQuizsSpecs(GetQuizsRequest request)
        : base(request) =>
        Query
            .Where(e => e.IsActive.Equals(request.IsActive!), request.IsActive.HasValue)
            .Where(e => e.QuizTypeId.Equals(request.QuizTypeId!), request.QuizTypeId.HasValue)
            .Where(e => e.QuizTopicId.Equals(request.QuizTopicId!), request.QuizTopicId.HasValue)
            .Where(e => e.QuizModeId.Equals(request.QuizModeId!), request.QuizModeId.HasValue)
            .Where(e => e.FromDate >= request.FromDate, request.FromDate.HasValue)
            .Where(e => e.ToDate <= request.ToDate, request.ToDate.HasValue)
            .OrderBy(e => e.Order);
}
