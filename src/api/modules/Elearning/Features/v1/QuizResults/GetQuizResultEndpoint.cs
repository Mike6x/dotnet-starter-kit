using FSH.Framework.Infrastructure.Auth.Policy;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Elearning.Features.v1.QuizResults;
public static class GetQuizResultEndpoint
{
    internal static RouteHandlerBuilder MapGetQuizResultEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/{id:guid}", async (Guid id, ISender mediator) =>
                        {
                            var response = await mediator.Send(new GetQuizResultRequest(id));
                            return Results.Ok(response);
                        })
                        .WithName(nameof(GetQuizResultEndpoint))
                        .WithSummary("gets QuizResult item by id")
                        .WithDescription("gets QuizResult item by id")
                        .Produces<GetQuizResultResponse>()
                        .RequirePermission("Permissions.QuizResults.View")
                        .MapToApiVersion(1);
    }
}
