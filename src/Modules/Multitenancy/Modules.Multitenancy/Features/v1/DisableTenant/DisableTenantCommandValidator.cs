using FluentValidation;
using FSH.Modules.Multitenancy.Contracts.v1.DisableTenant;

namespace FSH.Modules.Multitenancy.Features.v1.DisableTenant;

internal sealed class DisableTenantCommandValidator : AbstractValidator<DisableTenantCommand>
{
    public DisableTenantCommandValidator() =>
       RuleFor(t => t.TenantId)
           .NotEmpty();
}