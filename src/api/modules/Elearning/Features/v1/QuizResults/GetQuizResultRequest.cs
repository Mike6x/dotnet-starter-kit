using FSH.Framework.Core.Caching;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Elearning.Domain;
using FSH.Starter.WebApi.Elearning.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Starter.WebApi.Elearning.Features.v1.QuizResults;
public class GetQuizResultRequest(Guid id) : IRequest<GetQuizResultResponse>
{
    public Guid Id { get; set; } = id;
}

public sealed class GetQuizResultHandler(
    [FromKeyedServices("elearning:quizresults")] IReadRepository<QuizResult> repository,
    ICacheService cache)
    : IRequestHandler<GetQuizResultRequest, GetQuizResultResponse>
{
    public async Task<GetQuizResultResponse> Handle(GetQuizResultRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var item = await cache.GetOrSetAsync(
            $"QuizResult:{request.Id}",
            async () =>
            {
                var item = await repository.GetByIdAsync(request.Id, cancellationToken) 
                           ?? throw new QuizResultNotFoundException(request.Id);
                return new GetQuizResultResponse(
                    item.Id,
                    item.StartTime,
                    item.EndTime,

                    item.SId,
                    item.Sp,
                    item.Ut,
                    item.Fut,

                    item.Qt,
                    item.Tp,
                    item.Ps,
                    item.Psp,
                    item.Tl,

                    item.V,
                    item.T,
                    item.IsPass,
                    item.Rating,
                    item.QuizId
                );
            },
            cancellationToken: cancellationToken);
        return item!;
    }
}
