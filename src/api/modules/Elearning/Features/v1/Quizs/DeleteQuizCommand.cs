using FSH.Framework.Core.Caching;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Elearning.Domain;
using FSH.Starter.WebApi.Elearning.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Elearning.Features.v1.Quizs;

public sealed record DeleteQuizCommand(Guid Id) : IRequest;

// public sealed class DeleteQuizHandler(
//     ILogger<DeleteQuizHandler> logger,
//     [FromKeyedServices("elearning:quizs")] IRepository<Quiz> repository)
//     : IRequestHandler<DeleteQuizCommand>
// {
//     public async Task Handle(DeleteQuizCommand request, CancellationToken cancellationToken)
//     {
//         ArgumentNullException.ThrowIfNull(request);
//         var item = await repository.GetByIdAsync(request.Id, cancellationToken);
//         _ = item ?? throw new QuizNotFoundException(request.Id);
//         await repository.DeleteAsync(item, cancellationToken);
//         logger.LogInformation("Quiz item with id : {ItemId} deleted", item.Id);
//     }
// }
