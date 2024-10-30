using FSH.Framework.Core.Caching;
using FSH.Framework.Core.Domain.Events;
using FSH.Starter.WebApi.Elearning.Features.v1.QuizResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Elearning.Domain.Events;
public record QuizResultUpdated(QuizResult Item) : DomainEvent;

public class QuizResultUpdatedEventHandler(
    ILogger<QuizResultUpdatedEventHandler> logger,
    ICacheService cache)
    : INotificationHandler<QuizResultUpdated>
{
    public async Task Handle(QuizResultUpdated notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("handling QuizResult item update domain event..");

        var cacheResponse = new GetQuizResultResponse(
            notification.Item.Id,
            notification.Item.StartTime,
            notification.Item.EndTime,
            notification.Item.SId,
            notification.Item.Sp,
            notification.Item.Ut,
            notification.Item.Fut,
            notification.Item.Qt,
            notification.Item.Tp,
            notification.Item.Ps,
            notification.Item.Psp,
            notification.Item.Tl,
            notification.Item.V,
            notification.Item.T,
            notification.Item.IsPass,
            notification.Item.Rating,
            notification.Item.QuizId
            );

        await cache.SetAsync($"QuizResult:{notification.Item.Id}", cacheResponse, cancellationToken: cancellationToken);
    }
}
