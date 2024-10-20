using System.ComponentModel;
using FluentValidation;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Storage;
using FSH.Framework.Core.Storage.File;
using FSH.Framework.Core.Storage.File.Features;
using FSH.Starter.WebApi.Elearning.Domain;
using FSH.Starter.WebApi.Elearning.Persistence;
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

    [property: DefaultValue(null)]DateTime? FromDate,
    [property: DefaultValue(null)]DateTime? ToDate,
    Guid QuizTypeId,
    Guid QuizTopicId,
    Guid QuizModeId,
    [property: DefaultValue(0)] decimal? Price,
    [property: DefaultValue(null)] int? Sale,
    [property: DefaultValue(null)] int? RatingCount,
    [property: DefaultValue(null)] decimal? Rating,
    [property: DefaultValue(null)] FileUploadCommand? MediaUpload
    ) : IRequest<CreateQuizResponse>;

public record CreateQuizResponse(Guid? Id);

public sealed class CreateQuizHandler(
    IStorageService storageService,
    ILogger<CreateQuizHandler> logger,
    [FromKeyedServices("elearning:quizs")] IRepository<Quiz> repository)
    : IRequestHandler<CreateQuizCommand, CreateQuizResponse>
{
    public async Task<CreateQuizResponse> Handle(CreateQuizCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        // Remove old quiz if flag is set
        Uri? mediaUrl = null;
        Uri? quizPath = null;
        
        if (request.MediaUpload is not null) 
        {
            mediaUrl =  await storageService.UploadAsync<Quiz>(request.MediaUpload, FileType.QuizMedia, cancellationToken);
        }

        if (mediaUrl is not null)
        {
            quizPath = storageService.UnZip(mediaUrl);

            storageService.Remove(mediaUrl);
        }
        
        var item = Quiz.Create(
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

        await repository.AddAsync(item, cancellationToken).ConfigureAwait(false);
        await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        logger.LogInformation("Quiz item created {ItemId}", item.Id);
        return new CreateQuizResponse(item.Id);
    }
}

public class CreateQuizValidator : AbstractValidator<CreateQuizCommand>
{
    public CreateQuizValidator(ElearningDbContext context)
    {
        RuleFor(e => e.Code).NotEmpty().MinimumLength(2).MaximumLength(75);
        RuleFor(e => e.Name).NotEmpty().MinimumLength(2).MaximumLength(75);
        RuleFor(e => e.Price).GreaterThanOrEqualTo(0);
        RuleFor(e => e.QuizTopicId).NotEqual(Guid.Empty).WithMessage("QuizTopic required");
        RuleFor(e => e.QuizModeId).NotEqual(Guid.Empty).WithMessage("QuizMode Required");
        RuleFor(e => e.QuizTypeId).NotEqual(Guid.Empty).WithMessage("QuizType Required");;
    }
}
