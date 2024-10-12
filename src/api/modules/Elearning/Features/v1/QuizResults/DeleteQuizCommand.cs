using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Elearning.Domain;
using FSH.Starter.WebApi.Elearning.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Elearning.Features.v1.QuizResults;

public sealed record DeleteQuizResultCommand(Guid Id) : IRequest;

public sealed class DeleteQuizResultHandler(
    ILogger<DeleteQuizResultHandler> logger,
    [FromKeyedServices("elearning:quizresults")] IRepository<QuizResult> repository)
    : IRequestHandler<DeleteQuizResultCommand>
{
    public async Task Handle(DeleteQuizResultCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var item = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = item ?? throw new QuizResultNotFoundException(request.Id);
        await repository.DeleteAsync(item, cancellationToken);
        logger.LogInformation("QuizResult item with id : {ItemId} deleted", item.Id);
    }
}
