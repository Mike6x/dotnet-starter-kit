using FluentValidation;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Elearning.Domain;
using FSH.Starter.WebApi.Elearning.Exceptions;
using FSH.Starter.WebApi.Setting.Persistence;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Elearning.Features.v1.Quizs;
public sealed record UpdateQuizCommand(
    Guid Id,
    int? Order,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    string? QuizPath,
    DateTime? FromDate,
    DateTime? ToDate,
    Guid? QuizTypeId,
    Guid? QuizTopicId,
    Guid? QuizModeId,
    decimal? Price,
    int? Sale,
    int? RatingCount,
    decimal? Rating
    ) : IRequest<UpdateQuizResponse>;

public record UpdateQuizResponse(Guid? Id);

// public sealed class UpdateQuizHandler(
//     ILogger<UpdateQuizHandler> logger,
//     [FromKeyedServices("elearning:quiz")] IRepository<Quiz> repository)
//     : IRequestHandler<UpdateQuizCommand, UpdateQuizResponse>
// {
//     public async Task<UpdateQuizResponse> Handle(UpdateQuizCommand request, CancellationToken cancellationToken)
//     {
//         ArgumentNullException.ThrowIfNull(request);
//         var item = await repository.GetByIdAsync(request.Id, cancellationToken);
//         _ = item ?? throw new QuizNotFoundException(request.Id);
//
//         var updatedItem = item.Update(
//             request.Order,
//             request.Code,
//             request.Name,
//             request.Description,
//             request.IsActive,
//             request.QuizPath,
//             request.FromDate,
//             request.ToDate,
//             request.QuizTypeId,
//             request.QuizTopicId,
//             request.QuizModeId,
//             request.Price,
//             request.Sale,
//             request.RatingCount,
//             request.Rating
//         );
//
//         await repository.UpdateAsync(updatedItem, cancellationToken);
//         logger.LogInformation("Quiz Item updated {ItemId}", updatedItem.Id);
//         return new UpdateQuizResponse(updatedItem.Id);
//     }
// }

public class UpdateQuizValidator : AbstractValidator<UpdateQuizCommand>
{
    public UpdateQuizValidator(SettingDbContext context)
    {
        RuleFor(p => p.Code).NotEmpty();
        RuleFor(p => p.Name).NotEmpty();
   
    }
}

