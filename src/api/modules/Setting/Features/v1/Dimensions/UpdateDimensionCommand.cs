using MediatR;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Setting.Exceptions;
using FSH.Starter.WebApi.Setting.Domain;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
namespace FSH.Starter.WebApi.Setting.Features.v1.Dimensions;

public sealed record UpdateDimensionCommand(
    Guid Id,
    int? Order,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    string? FullName,
    string? NativeName,
    string? FullNativeName,
    int? Value,
    string Type,
    Guid FatherId
    ) : IRequest<UpdateDimensionResponse>;

public record UpdateDimensionResponse(Guid? Id);

public sealed class UpdateDimensionHandler(
    ILogger<UpdateDimensionHandler> logger,
    [FromKeyedServices("setting:dimension")] IRepository<Dimension> repository)
    : IRequestHandler<UpdateDimensionCommand, UpdateDimensionResponse>
{
    public async Task<UpdateDimensionResponse> Handle(UpdateDimensionCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var item = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new DimensionNotFoundException(request.Id);

        var updatedItem = item.Update(
            request.Order,
            request.Code,
            request.Name,
            request.Description,
            request.IsActive,
            request.FullName,
            request.NativeName,
            request.FullNativeName,
            request.Value,
            request.Type,
            request.FatherId);

        await repository.UpdateAsync(updatedItem, cancellationToken);
        logger.LogInformation("Dimension Item updated {ItemId}", updatedItem.Id);
        return new UpdateDimensionResponse(updatedItem.Id);
    }
}

public class UpdateDimensionValidator : AbstractValidator<UpdateDimensionCommand>
{
    public UpdateDimensionValidator([FromKeyedServices("setting:dimension")] IRepository<Dimension> repository)
    {
        RuleFor(e => e.Code)
            .NotEmpty().MinimumLength(2).MaximumLength(75)
            .MustAsync(async (item, code, ct) => await repository.FirstOrDefaultAsync(new DimensionByCodeSpec(code), ct) 
                is not { } existingItem || existingItem.Id == item.Id)
            .WithMessage((_, code) => $"Item with Code: {code} already exists.");

        RuleFor(e => e.Name)
            .NotEmpty().MinimumLength(2).MaximumLength(75)
            .MustAsync(async (item, name, ct) => await repository.FirstOrDefaultAsync(new DimensionByNameSpec(name), ct) 
                is not { } existingItem || existingItem.Id == item.Id)
            .WithMessage((_, name) => $"Item with Name: {name} already exists.");
        
        RuleFor(e => e.Type).NotEmpty();
    }
}
