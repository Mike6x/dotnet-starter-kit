using Asp.Versioning;
using FSH.Framework.Infrastructure.Auth.Policy;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Elearning.Features.v1.QuizResults;
public static class DeleteQuizResultEndpoint
{
    internal static RouteHandlerBuilder MapDeleteQuizResultEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapDelete("/{id:guid}", async (Guid id, ISender mediator) =>
            {
                await mediator.Send(new QuizResults.DeleteQuizResultCommand(id));
                return Results.NoContent();
            })
            .WithName(nameof(DeleteQuizResultEndpoint))
            .WithSummary("Deletes a QuizResult item")
            .WithDescription("Deleted a QuizResult item")
            .Produces(StatusCodes.Status204NoContent)
            .RequirePermission("Permissions.QuizResults.Delete")
            .MapToApiVersion(new ApiVersion(1, 0));

    }
}
