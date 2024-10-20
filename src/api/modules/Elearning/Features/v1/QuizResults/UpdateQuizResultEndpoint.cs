using Asp.Versioning;
using FSH.Framework.Infrastructure.Auth.Policy;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Elearning.Features.v1.QuizResults;
public static class UpdateQuizResultEndpoint
{
    internal static RouteHandlerBuilder MapUpdateQuizResultEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.
            MapPut("/{id:guid}", async (Guid id, QuizResults.UpdateQuizResultCommand request, ISender mediator) =>
            {
                if (id != request.Id) return Results.BadRequest();
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(UpdateQuizResultEndpoint))
            .WithSummary("Updates a QuizResult item")
            .WithDescription("Updated a QuizResult item")
            .Produces<QuizResults.UpdateQuizResultResponse>()
            .RequirePermission("Permissions.QuizResults.Update")
            .MapToApiVersion(new ApiVersion(1, 0));

    }
}
