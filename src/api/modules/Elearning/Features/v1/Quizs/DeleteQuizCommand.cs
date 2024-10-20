using FSH.Framework.Core.Exceptions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Storage;
using FSH.Starter.WebApi.Elearning.Domain;
using FSH.Starter.WebApi.Elearning.Exceptions;
using FSH.Starter.WebApi.Elearning.Features.v1.QuizResults;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Elearning.Features.v1.Quizs;

public sealed record DeleteQuizCommand(Guid Id) : IRequest;

public sealed class DeleteQuizHandler(
    IStorageService storageService,
    ILogger<DeleteQuizHandler> logger,
     [FromKeyedServices("elearning:quizresults")] IRepository<QuizResult> quizResultRepo,
    [FromKeyedServices("elearning:quizs")] IRepository<Quiz> repository)
    : IRequestHandler<DeleteQuizCommand>
{
    public async Task Handle(DeleteQuizCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var item = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = item ?? throw new QuizNotFoundException(request.Id);

        if (await quizResultRepo.AnyAsync(new QuizResultsByQuizIdSpec(request.Id), cancellationToken))
        {
            throw new ConflictException("Item cannot be deleted as it's being used.");
        }

        if (item.QuizUrl != null)
            {
                storageService.RemoveFolder(storageService.GetLocalPathFromUri(item.QuizUrl!, true));
            }
        await repository.DeleteAsync(item, cancellationToken);
        logger.LogInformation("Quiz item with id : {ItemId} deleted", item.Id);
    }
}


