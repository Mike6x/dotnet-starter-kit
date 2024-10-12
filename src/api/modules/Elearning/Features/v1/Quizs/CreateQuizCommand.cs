using System.ComponentModel;
using FluentValidation;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Elearning.Domain;
using FSH.Starter.WebApi.Setting.Persistence;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Elearning.Features.v1.Quizs;
public record CreateQuizCommand(
    [property: DefaultValue(0)] int Order,
    [property: DefaultValue("string.Empty")] string Code,
    [property: DefaultValue("string.Empty")] string Name,
    [property: DefaultValue(null)] string? Description,
    [property: DefaultValue(true)] bool IsActive,
    [property: DefaultValue("string.Empty")] string QuizPath, 
    DateTime? FromDate,
    DateTime? ToDate,
    Guid QuizTypeId,
    Guid QuizTopicId,
    Guid QuizModeId,
    [property: DefaultValue(0)] decimal? Price,
    [property: DefaultValue(null)] int? Sale,
    [property: DefaultValue(null)] int? RatingCount,
    [property: DefaultValue(null)] decimal? Rating
    ) : IRequest<CreateQuizResponse>;

public record CreateQuizResponse(Guid? Id);
//
// public sealed class CreateQuizHandler(
//     ILogger<CreateQuizHandler> logger,
//     [FromKeyedServices("elearning:quiz")] IRepository<Quiz> repository)
//     : IRequestHandler<CreateQuizCommand, CreateQuizResponse>
// {
//     public async Task<CreateQuizResponse> Handle(CreateQuizCommand request, CancellationToken cancellationToken)
//     {
//         ArgumentNullException.ThrowIfNull(request);
//         var item = Quiz.Create(
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
//         await repository.AddAsync(item, cancellationToken).ConfigureAwait(false);
//         await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
//         logger.LogInformation("Quiz item created {ItemId}", item.Id);
//         return new CreateQuizResponse(item.Id);
//     }
// }

public class CreateQuizValidator : AbstractValidator<CreateQuizCommand>
{
    public CreateQuizValidator(SettingDbContext context)
    {
        RuleFor(p => p.Code).NotEmpty();
        RuleFor(p => p.Name).NotEmpty();
    }
}
