using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Setting.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using FluentValidation;

namespace FSH.Starter.WebApi.Setting.Features.v1.Dimensions;
public record CreateDimensionCommand(
    [property: DefaultValue(0)] int Order,
    [property: DefaultValue("string.Empty")] string Code,
    [property: DefaultValue("string.Empty")] string Name,
    [property: DefaultValue(null)] string? Description,
    [property: DefaultValue(true)] bool IsActive,
    [property: DefaultValue(null)] string? FullName,
    [property: DefaultValue(null)] string? NativeName,
    [property: DefaultValue(null)] string? FullNativeName,
    [property: DefaultValue(0)] int? Value,
    [property: DefaultValue("string.Empty")] string Type,
    Guid? FatherId
    ) : IRequest<CreateDimensionResponse>;

public record CreateDimensionResponse(Guid? Id);


public sealed class CreateDimensionHandler(
    ILogger<CreateDimensionHandler> logger,
    [FromKeyedServices("setting:dimension")] IRepository<Dimension> repository)
    : IRequestHandler<CreateDimensionCommand, CreateDimensionResponse>
{
    public async Task<CreateDimensionResponse> Handle(CreateDimensionCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var item = Dimension.Create(
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

        await repository.AddAsync(item, cancellationToken).ConfigureAwait(false);
        await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        logger.LogInformation("Dimension item created {ItemId}", item.Id);
        return new CreateDimensionResponse(item.Id);
    }
}

public class CreateDimensionValidator : AbstractValidator<CreateDimensionCommand>
{
    public CreateDimensionValidator([FromKeyedServices("setting:dimension")] IRepository<Dimension> repository)
    {
        RuleFor(e => e.Code)
            .NotEmpty().MinimumLength(2).MaximumLength(75)
            .MustAsync(async (code, ct) => await repository.FirstOrDefaultAsync(new DimensionByCodeSpec(code), ct) is null)
            .WithMessage((_, code) => $"Item with Code: {code} already exists.");

        RuleFor(e => e.Name)
            .NotEmpty().MinimumLength(2).MaximumLength(75)
            .MustAsync(async (name, ct) => await repository.FirstOrDefaultAsync(new DimensionByNameSpec(name), ct) is null)
            .WithMessage((_, name) => $"Item with Name: {name} already exists.");

        RuleFor(e => e.Type).NotEmpty();
    }
}
