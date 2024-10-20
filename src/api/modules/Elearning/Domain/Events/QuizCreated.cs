using FSH.Framework.Core.Caching;
using FSH.Framework.Core.Domain.Events;
using FSH.Starter.WebApi.Elearning.Features.v1.Quizs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Elearning.Domain.Events;
public record QuizCreated(
    Guid Id,
    int? Order,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
        
    Uri? QuizUrl,
    DateTime? FromDate,
    DateTime? ToDate,
        
    Guid QuizTypeId,
    Guid QuizTopicId,
    Guid QuizModeId,
        
    decimal? Price,
    int? Sale,
    int? RatingCount,
    decimal? Rating
    ) : DomainEvent;

public class QuizCreatedEventHandler(
    ILogger<QuizCreatedEventHandler> logger,
    ICacheService cache)
    : INotificationHandler<QuizCreated>
{
    public async Task Handle(QuizCreated notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("handling Quiz created domain event..");
        var cacheResponse = new GetQuizResponse(
            notification.Id,
            notification.Order,
            notification.Code,
            notification.Name,
            notification.Description,
            notification.IsActive,
            notification.QuizUrl,
            notification.FromDate,
            notification.ToDate,
            notification.QuizTypeId,
            notification.QuizTopicId,
            notification.QuizModeId,
            notification.Price,
            notification.Sale,
            notification.RatingCount,
            notification.Rating
            );
        await cache.SetAsync($"Quiz:{notification.Id}", cacheResponse, cancellationToken: cancellationToken);
    }
}
