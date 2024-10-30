using Asp.Versioning;
using FSH.Framework.Infrastructure.Auth.Policy;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Elearning.Features.v1.QuizResults;
public static class CreateQuizResultEndpoint
{
    internal static RouteHandlerBuilder MapCreateQuizResultEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/", async (CreateQuizResultCommand request, ISender mediator) =>
                {
                    var response = await mediator.Send(request);
                    return Results.CreatedAtRoute(nameof(CreateQuizResultEndpoint), new { id = response.Id }, response);
                })
                .WithName(nameof(CreateQuizResultEndpoint))
                .WithSummary("Creates a QuizResult item")
                .WithDescription("Creates a QuizResult item")
                .Produces<CreateQuizResultResponse>(StatusCodes.Status201Created)
                .RequirePermission("Permissions.QuizResults.Create")
                .MapToApiVersion(new ApiVersion(1, 0));

    }
}
