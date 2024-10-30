using System.ComponentModel;
using FluentValidation;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Elearning.Domain;
using FSH.Starter.WebApi.Elearning.Persistence;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Elearning.Features.v1.QuizResults;
public record MobileCreateQuizResultCommand(
        
    [property: DefaultValue(null)] string? SId,
    [property: DefaultValue(0)] decimal Sp,
    [property: DefaultValue(0)] decimal Ut,
    [property: DefaultValue("string.Empty")] string Fut, 
    
    [property: DefaultValue("string.Empty")] string Qt, 
    [property: DefaultValue(100)] decimal Tp,
    [property: DefaultValue(100)] decimal Ps,
    [property: DefaultValue(100)] decimal Psp,
    [property: DefaultValue(0)] decimal Tl,

    [property: DefaultValue("9.0")] string V, 
    [property: DefaultValue("Graded")] string T, 

    // [property: DefaultValue(null)] decimal? Rating,

    Guid QuizId

   ) : IRequest<CreateQuizResultResponse>;


public sealed class MobileCreateQuizResultHandler(
    ILogger<MobileCreateQuizResultHandler> logger,
    [FromKeyedServices("elearning:quizresults")] IRepository<QuizResult> repository)
    : IRequestHandler<MobileCreateQuizResultCommand, CreateQuizResultResponse>
{
    public async Task<CreateQuizResultResponse> Handle(MobileCreateQuizResultCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var item = QuizResult.Create(
            DateTime.UtcNow - TimeSpan.FromSeconds((double)request.Ut),
            DateTime.UtcNow,

            request.SId,
            request.Sp,
            request.Ut,
            request.Fut ?? string.Empty,

            request.Qt,
            request.Tp,
            request.Ps,
            request.Psp,
            request.Tl,

            request.V ?? "9.0",
            request.T ?? "Graded",
            
            null,
            request.QuizId
        );

        await repository.AddAsync(item, cancellationToken).ConfigureAwait(false);
        await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        logger.LogInformation("QuizResult item created {ItemId}", item.Id);
        return new CreateQuizResultResponse(item.Id);
    }
}

public class CreateMobileQuizResultValidator : AbstractValidator<MobileCreateQuizResultCommand>
{
    public CreateMobileQuizResultValidator(ElearningDbContext context)
    {
        RuleFor(e => e.QuizId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await context.Quizs.FindAsync([id], cancellationToken: ct) is not null)
                .WithMessage((_, id) => $"Quiz with {id} not existed.");
        RuleFor(e => e.Qt).NotEmpty();
        RuleFor(e => e.Sp).GreaterThanOrEqualTo(0);
        RuleFor(e => e.Ps).GreaterThanOrEqualTo(0);
        RuleFor(e => e.Psp).GreaterThanOrEqualTo(0);
        RuleFor(e => e.Tp).GreaterThanOrEqualTo(0);
        RuleFor(e => e.Ut).GreaterThanOrEqualTo(0);

    }
}
