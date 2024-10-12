using Asp.Versioning;
using FSH.Framework.Infrastructure.Auth.Policy;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Elearning.Features.v1.Quizs;
public static class CreateQuizEndpoint
{
    internal static RouteHandlerBuilder MapCreateQuizEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/", async (CreateQuizCommand request, ISender mediator) =>
                {
                    var response = await mediator.Send(request);
                    return Results.CreatedAtRoute(nameof(CreateQuizEndpoint), new { id = response.Id }, response);
                })
                .WithName(nameof(CreateQuizEndpoint))
                .WithSummary("Creates a Quiz item")
                .WithDescription("Creates a Quiz item")
                .Produces<CreateQuizResponse>(StatusCodes.Status201Created)
                .RequirePermission("Permissions.Quizs.Create")
                .MapToApiVersion(new ApiVersion(1, 0));

    }
}
