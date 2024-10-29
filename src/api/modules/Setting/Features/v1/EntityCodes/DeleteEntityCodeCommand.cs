using MediatR;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Setting.Exceptions;
using FSH.Starter.WebApi.Setting.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FSH.Framework.Core.Exceptions;

namespace FSH.Starter.WebApi.Setting.Features.v1.EntityCodes;

public sealed record DeleteEntityCodeCommand( Guid Id) : IRequest;

public sealed class DeleteEntityCodeHandler(
    ILogger<DeleteEntityCodeHandler> logger,
    [FromKeyedServices("setting:EntityCode")] IRepository<EntityCode> repository)
    : IRequestHandler<DeleteEntityCodeCommand>
{
    public async Task Handle(DeleteEntityCodeCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var item = await repository.GetByIdAsync(request.Id, cancellationToken)
                    ?? throw new NotFoundException($"item with id {request.Id} not found");
                    
        await repository.DeleteAsync(item, cancellationToken);
        logger.LogInformation("EntityCode item with id : {ItemId} deleted", item.Id);
    }
}




