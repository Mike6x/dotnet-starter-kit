using Asp.Versioning;
using FSH.Framework.Infrastructure.Auth.Policy;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Elearning.Features.v1.Quizs;
public static class DeleteQuizEndpoint
{
    internal static RouteHandlerBuilder MapDeleteQuizEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapDelete("/{id:guid}", async (Guid id, ISender mediator) =>
            {
                await mediator.Send(new DeleteQuizCommand(id));
                return Results.NoContent();
            })
            .WithName(nameof(DeleteQuizEndpoint))
            .WithSummary("Deletes a Quiz item")
            .WithDescription("Deleted a Quiz item")
            .Produces(StatusCodes.Status204NoContent)
            .RequirePermission("Permissions.Quizs.Delete")
            .MapToApiVersion(new ApiVersion(1, 0));

    }
}
