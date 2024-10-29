using System.ComponentModel;
using FluentValidation;
using FSH.Starter.WebApi.Setting.Domain;
using FSH.Starter.WebApi.Setting.Persistence;
using MediatR;
using FSH.Framework.Core.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Setting.Features.v1.EntityCodes;

public record CreateEntityCodeCommand(
    [property: DefaultValue(0)] int Order,
    [property: DefaultValue("string.Empty")] string Code,
    [property: DefaultValue("")] string Name,
    [property: DefaultValue(null)] string? Description,
    [property: DefaultValue(true)] bool? IsActive,

    [property: DefaultValue(null)] string? Separator,
    [property: DefaultValue(0)] int? Value,
    [property: DefaultValue(CodeType.MasterData)] CodeType Type
    ) : IRequest<CreateEntityCodeResponse>;


public record CreateEntityCodeResponse(Guid? Id);

public sealed class CreateEntityCodeHandler(
    ILogger<CreateEntityCodeHandler> logger,
    [FromKeyedServices("setting:EntityCode")] IRepository<EntityCode> repository)
    : IRequestHandler<CreateEntityCodeCommand, CreateEntityCodeResponse>
{
    public async Task<CreateEntityCodeResponse> Handle(CreateEntityCodeCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var item = EntityCode.Create(
            request.Order,
            request.Code,
            request.Name,
            request.Description,
            request.IsActive,
            request.Separator,
            request.Value,
            request.Type);

        await repository.AddAsync(item, cancellationToken).ConfigureAwait(false);
        await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        logger.LogInformation("EntityCode item created {ItemId}", item.Id);
        return new CreateEntityCodeResponse(item.Id);
    }
}


public class CreateEntityCodeValidator : AbstractValidator<CreateEntityCodeCommand>
{
    public CreateEntityCodeValidator(SettingDbContext context)
    {
        RuleFor(p => p.Code).NotEmpty();
        RuleFor(p => p.Name).NotEmpty();
    }
}
