using FSH.Framework.Core.Caching;
using FSH.Framework.Core.Domain.Events;
using FSH.Starter.WebApi.Elearning.Features.v1.Quizs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Elearning.Domain.Events;
public record QuizUpdated(Quiz Item) : DomainEvent;

public class QuizUpdatedEventHandler(
    ILogger<QuizUpdatedEventHandler> logger,
    ICacheService cache)
    : INotificationHandler<QuizUpdated>
{
    public async Task Handle(QuizUpdated notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("handling Quiz item update domain event..");

        var cacheResponse = new GetQuizResponse(
             notification.Item.Id,
             notification.Item.Order,
             notification.Item.Code,
             notification.Item.Name,
             notification.Item.Description,
             notification.Item.IsActive,
             notification.Item.QuizPath,
             notification.Item.FromDate,
             notification.Item.ToDate,
             notification.Item.QuizTypeId,
             notification.Item.QuizTopicId,
             notification.Item.QuizModeId,
             notification.Item.Price,
             notification.Item.Sale,
             notification.Item.RatingCount,
             notification.Item.Rating
            );

        await cache.SetAsync($"Quiz:{notification.Item.Id}", cacheResponse, cancellationToken: cancellationToken);
    }
}
