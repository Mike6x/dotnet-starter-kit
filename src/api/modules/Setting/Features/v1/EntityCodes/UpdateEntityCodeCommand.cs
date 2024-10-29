using FSH.Starter.WebApi.Setting.Domain;
using MediatR;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Setting.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FluentValidation;
using FSH.Starter.WebApi.Setting.Persistence;
using FSH.Framework.Core.Exceptions;

namespace FSH.Starter.WebApi.Setting.Features.v1.EntityCodes;

public sealed record UpdateEntityCodeCommand(
    Guid Id,
    int? Order,    
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    string? Separator,
    int? Value,
    CodeType Type
    ) : IRequest<UpdateEntityCodeResponse>;

public record UpdateEntityCodeResponse(Guid? Id);


public sealed class UpdateEntityCodeHandler(
    ILogger<UpdateEntityCodeHandler> logger,
    [FromKeyedServices("setting:EntityCode")] IRepository<EntityCode> repository)
    : IRequestHandler<UpdateEntityCodeCommand, UpdateEntityCodeResponse>
{
    public async Task<UpdateEntityCodeResponse> Handle(UpdateEntityCodeCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var item = await repository.GetByIdAsync(request.Id, cancellationToken)        
            ?? throw new NotFoundException($"item with id {request.Id} not found");

        var updatedItem = item.Update(
            request.Order,
            request.Code,
            request.Name,
            request.Description,
            request.IsActive,
            request.Separator,
            request.Value,
            request.Type);

        await repository.UpdateAsync(updatedItem, cancellationToken);
        logger.LogInformation("EntityCode Item updated {ItemId}", updatedItem.Id);
        return new UpdateEntityCodeResponse(updatedItem.Id);
    }
}

public class UpdateEntityCodeValidator : AbstractValidator<UpdateEntityCodeCommand>
{
    public UpdateEntityCodeValidator(SettingDbContext context)
    {
        RuleFor(p => p.Code).NotEmpty();
        RuleFor(p => p.Name).NotEmpty();
    }
}
