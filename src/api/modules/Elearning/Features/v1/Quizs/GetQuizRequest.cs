using FSH.Framework.Core.Caching;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Elearning.Domain;
using FSH.Starter.WebApi.Elearning.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Starter.WebApi.Elearning.Features.v1.Quizs;
public class GetQuizRequest : IRequest<GetQuizResponse>
{
    public Guid Id { get; set; }
    public GetQuizRequest(Guid id) => Id = id;
}

public record GetQuizResponse(
    Guid Id,
    int? Order,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    string QuizPath,
    DateTime? FromDate,
    DateTime? ToDate,
    Guid QuizTypeId,
    Guid QuizTopicId,
    Guid QuizModeId,
    decimal? Price,
    int? Sale,
    int? RatingCount,
    decimal? Rating);

// public sealed class GetQuizHandler(
//     [FromKeyedServices("elearning:quiz")] IReadRepository<Quiz> repository,
//     ICacheService cache)
//     : IRequestHandler<GetQuizRequest, GetQuizResponse>
// {
//     public async Task<GetQuizResponse> Handle(GetQuizRequest request, CancellationToken cancellationToken)
//     {
//         ArgumentNullException.ThrowIfNull(request);
//         var item = await cache.GetOrSetAsync(
//             $"Quiz:{request.Id}",
//             async () =>
//             {
//                 var item = await repository.GetByIdAsync(request.Id, cancellationToken);
//                 if (item == null) throw new QuizNotFoundException(request.Id);
//                 return new GetQuizResponse(
//                     item.Id,
//                     item.Order,
//                     item.Code!,
//                     item.Name!,
//                     item.Description,
//                     item.IsActive,
//                     item.QuizPath,
//                     item.FromDate,
//                     item.ToDate,
//                     item.QuizTypeId,
//                     item.QuizTopicId,
//                     item.QuizModeId,
//                     item.Price,
//                     item.Sale,
//                     item.RatingCount,
//                     item.Rating
//                 );
//             },
//             cancellationToken: cancellationToken);
//         return item!;
//     }
// }
