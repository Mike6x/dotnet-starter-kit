using Asp.Versioning;
using FSH.Framework.Infrastructure.Auth.Policy;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Elearning.Features.v1.Quizs;
public static class UpdateQuizEndpoint
{
    internal static RouteHandlerBuilder MapUpdateQuizEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.
            MapPut("/{id:guid}", async (Guid id, UpdateQuizCommand request, ISender mediator) =>
            {
                if (id != request.Id) return Results.BadRequest();
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(UpdateQuizEndpoint))
            .WithSummary("Updates a Quiz item")
            .WithDescription("Updated a Quiz item")
            .Produces<UpdateQuizResponse>()
            .RequirePermission("Permissions.Quizs.Update")
            .MapToApiVersion(new ApiVersion(1, 0));

    }
}
