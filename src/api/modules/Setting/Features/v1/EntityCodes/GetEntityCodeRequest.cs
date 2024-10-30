using MediatR;
using FSH.Framework.Core.Caching;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Setting.Domain;
using Microsoft.Extensions.DependencyInjection;
using FSH.Framework.Core.Exceptions;

namespace FSH.Starter.WebApi.Setting.Features.v1.EntityCodes;

public class GetEntityCodeRequest(Guid id) : IRequest<GetEntityCodeResponse>
{
    public Guid Id { get; set; } = id;
}


public sealed class GetEntityCodeHandler(
    [FromKeyedServices("setting:EntityCode")] IReadRepository<EntityCode> repository,
    ICacheService cache)
    : IRequestHandler<GetEntityCodeRequest, GetEntityCodeResponse>
{
    public async Task<GetEntityCodeResponse> Handle(GetEntityCodeRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var item = await cache.GetOrSetAsync(
            $"EntityCode:{request.Id}",
            async () =>
            {
                var item = await repository.GetByIdAsync(request.Id, cancellationToken)
                    ?? throw new NotFoundException($"item with id {request.Id} not found");

                return new GetEntityCodeResponse(
                    item.Id,
                    item.Order,
                    item.Code!,
                    item.Name!,
                    item.Description,
                    item.IsActive,
                    item.Separator,
                    item.Value,
                    item.Type);
            },
            cancellationToken: cancellationToken);
        return item!;
    }
}
