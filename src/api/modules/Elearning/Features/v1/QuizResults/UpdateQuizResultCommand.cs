using FluentValidation;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Elearning.Domain;
using FSH.Starter.WebApi.Elearning.Exceptions;
using FSH.Starter.WebApi.Elearning.Persistence;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Elearning.Features.v1.QuizResults;
public sealed record UpdateQuizResultCommand(
    Guid Id,
    DateTime? StartTime,
    DateTime? EndTime,
    
    string? SId,
    decimal? Sp,
    decimal? Ut,
    string? Fut, 
    
    string? Qt, 
    decimal? Tp,
    decimal? Ps,
    decimal? Psp,
    decimal ?Tl,

    string? V, 
    string? T, 
    
    decimal? Rating,
    Guid QuizId
    ) : IRequest<UpdateQuizResultResponse>;

public record UpdateQuizResultResponse(Guid? Id);

public sealed class UpdateQuizResultHandler(
    ILogger<UpdateQuizResultHandler> logger,
    [FromKeyedServices("elearning:quizresults")] IRepository<QuizResult> repository)
    : IRequestHandler<UpdateQuizResultCommand, UpdateQuizResultResponse>
{
    public async Task<UpdateQuizResultResponse> Handle(UpdateQuizResultCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var item = await repository.GetByIdAsync(request.Id, cancellationToken) ?? throw new QuizResultNotFoundException(request.Id);

        var updatedItem = item.Update(
            request.StartTime ?? DateTime.UtcNow - TimeSpan.FromSeconds((double)request.Ut!),
            request.EndTime ?? DateTime.UtcNow,

            request.SId,
            request.Sp,
            request.Ut,
            request.Fut,

            request.Qt,
            request.Tp,
            request.Ps,
            request.Psp,
            request.Tl,

            request.V,
            request.T,
            
            request.Rating,
            request.QuizId
        );

        await repository.UpdateAsync(updatedItem, cancellationToken);
        logger.LogInformation("QuizResult Item updated {ItemId}", updatedItem.Id);
        return new UpdateQuizResultResponse(updatedItem.Id);
    }
}

public class UpdateQuizResultValidator : AbstractValidator<UpdateQuizResultCommand>
{
    public UpdateQuizResultValidator(ElearningDbContext context)
    {
        RuleFor(e => e.Sp).GreaterThanOrEqualTo(0);
        RuleFor(e => e.Ps).GreaterThanOrEqualTo(0);
        RuleFor(e => e.Psp).GreaterThanOrEqualTo(0);
        RuleFor(e => e.Tp).GreaterThanOrEqualTo(0);
        RuleFor(e => e.QuizId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await context.Quizs.FindAsync([id], cancellationToken: ct) is not null)
                .WithMessage((_, id) => $"Quiz with {id} existed.");

    }

}
