using FSH.Framework.Core.Caching;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Elearning.Domain;
using FSH.Starter.WebApi.Elearning.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Starter.WebApi.Elearning.Features.v1.Quizs;
public class GetQuizRequest(Guid id) : IRequest<GetQuizResponse>
{
    public Guid Id { get; set; } = id;
}



public sealed class GetQuizHandler(
    [FromKeyedServices("elearning:quizs")] IReadRepository<Quiz> repository,
    ICacheService cache)
    : IRequestHandler<GetQuizRequest, GetQuizResponse>
{
    public async Task<GetQuizResponse> Handle(GetQuizRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var item = await cache.GetOrSetAsync(
            $"Quiz:{request.Id}",
            async () =>
            {
                var item = await repository.GetByIdAsync(request.Id, cancellationToken)
                           ?? throw new QuizNotFoundException(request.Id) ;

                return new GetQuizResponse(
                    item.Id,
                    item.Order,
                    item.Code!,
                    item.Name!,
                    item.Description,
                    item.IsActive,
                    item.QuizUrl,
                    item.FromDate,
                    item.ToDate,
                    item.QuizTypeId,
                    item.QuizTopicId,
                    item.QuizModeId,
                    item.Price,
                    item.Sale,
                    item.RatingCount,
                    item.Rating
                );
            },
            cancellationToken: cancellationToken);
        return item!;
    }
}
