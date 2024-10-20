using FluentValidation;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Storage;
using FSH.Framework.Core.Storage.File;
using FSH.Framework.Core.Storage.File.Features;
using FSH.Starter.WebApi.Elearning.Domain;
using FSH.Starter.WebApi.Elearning.Exceptions;
using FSH.Starter.WebApi.Elearning.Persistence;
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
    Uri? QuizUrl,
    DateTime? FromDate,
    DateTime? ToDate,
    Guid QuizTypeId,
    Guid QuizTopicId,
    Guid QuizModeId,
    decimal? Price,
    int? Sale,
    int? RatingCount,
    decimal? Rating,
    FileUploadCommand? MediaUpload,
    bool DeleteCurrentQuiz 
    ) : IRequest<UpdateQuizResponse>;

public record UpdateQuizResponse(Guid? Id);

public sealed class UpdateQuizHandler(
    IStorageService storageService,
    ILogger<UpdateQuizHandler> logger,
    [FromKeyedServices("elearning:quizs")] IRepository<Quiz> repository)
    : IRequestHandler<UpdateQuizCommand, UpdateQuizResponse>
{
    public async Task<UpdateQuizResponse> Handle(UpdateQuizCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var item = await repository.GetByIdAsync(request.Id, cancellationToken)
                   ?? throw new QuizNotFoundException(request.Id);
        
        // Remove old quiz if flag is set
        Uri? quizPath = null;

        if (request.MediaUpload != null || request.DeleteCurrentQuiz)
        {
            if (request.DeleteCurrentQuiz && item.QuizUrl != null)
            {
                // storageService.Remove(item.QuizUrl)
                storageService.RemoveFolder(storageService.GetLocalPathFromUri(item.QuizUrl!, true));
            }
            
            var mediaUrl = await storageService.UploadAsync<Quiz>(request.MediaUpload, FileType.QuizMedia, cancellationToken);
            quizPath = storageService.UnZip(mediaUrl);
            storageService.Remove(mediaUrl);
        }
        
        var updatedItem = item.Update(
            request.Order,
            request.Code,
            request.Name,
            request.Description,
            request.IsActive,
            quizPath,
            request.FromDate,
            request.ToDate,
            request.QuizTypeId,
            request.QuizTopicId,
            request.QuizModeId,
            request.Price,
            request.Sale,
            request.RatingCount,
            request.Rating
        );

        await repository.UpdateAsync(updatedItem, cancellationToken);
        logger.LogInformation("Quiz Item updated {ItemId}", updatedItem.Id);
        return new UpdateQuizResponse(updatedItem.Id);
    }
}

public class UpdateQuizValidator : AbstractValidator<UpdateQuizCommand>
{
    public UpdateQuizValidator(ElearningDbContext context)
    {
        RuleFor(e => e.Code).NotEmpty().MinimumLength(2).MaximumLength(75);
        RuleFor(e => e.Name).NotEmpty().MinimumLength(2).MaximumLength(75);
        RuleFor(e => e.Price).GreaterThanOrEqualTo(0);
        RuleFor(e => e.Sale).GreaterThanOrEqualTo(0);
        RuleFor(e => e.Rating).GreaterThanOrEqualTo(0);
        RuleFor(e => e.RatingCount).GreaterThanOrEqualTo(0);

    }
}

