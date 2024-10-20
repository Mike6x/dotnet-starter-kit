using FSH.Framework.Core.Caching;
using FSH.Framework.Core.Domain.Events;
using FSH.Starter.WebApi.Elearning.Features.v1.QuizResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Elearning.Domain.Events;
public record QuizResultCreated(
    Guid Id,
    DateTime startTime,
    DateTime endTime,
    string? userId,
    decimal earnedPoints,
    decimal userTimeSpend,
    string timeTakingQuiz,
    string quizTitle,
    decimal gainedScore,
    decimal passingScore,
    decimal passingScoreInPercent,
    decimal timeLimit,
    string quizVersion,
    string quizType,
    bool isPass,
    decimal? rating,
    Guid quizId
) : DomainEvent;

public class QuizResultCreatedEventHandler(
    ILogger<QuizResultCreatedEventHandler> logger,
    ICacheService cache)
    : INotificationHandler<QuizResultCreated>
{
    public async Task Handle(QuizResultCreated notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("handling QuizResult created domain event..");
        var cacheResponse = new GetQuizResultResponse(
            notification.Id,
            notification.startTime,
            notification.endTime,
            notification.userId,
            notification.earnedPoints,
            notification.userTimeSpend,
            notification.timeTakingQuiz,
            notification.quizTitle,
            notification.gainedScore,
            notification.passingScore,
            notification.passingScoreInPercent,
            notification.timeLimit,
            notification.quizVersion,
            notification.quizType,
            notification.isPass,
            notification.rating,
            notification.quizId
        );
        await cache.SetAsync($"QuizResult:{notification.Id}", cacheResponse, cancellationToken: cancellationToken);
    }
}
