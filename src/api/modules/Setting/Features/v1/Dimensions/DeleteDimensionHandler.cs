using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Setting.Exceptions;
using FSH.Starter.WebApi.Setting.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FSH.Framework.Core.Exceptions;

namespace FSH.Starter.WebApi.Setting.Features.v1.Dimensions;
public sealed class DeleteDimensionHandler(
    ILogger<DeleteDimensionHandler> logger,
    [FromKeyedServices("setting:dimension")] IRepository<Dimension> repository)
    : IRequestHandler<DeleteDimensionCommand>
{
    public async Task Handle(DeleteDimensionCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var item = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = item ?? throw new DimensionNotFoundException(request.Id);

        if (await repository.AnyAsync(new DimensionByFatherIdSpec(item.Id), cancellationToken)) 
        {
            throw new ConflictException("Item cannot be deleted as it's being used.");
        }
        
        await repository.DeleteAsync(item, cancellationToken);
        logger.LogInformation("Dimension item with id : {ItemId} deleted", item.Id);
    }
}
