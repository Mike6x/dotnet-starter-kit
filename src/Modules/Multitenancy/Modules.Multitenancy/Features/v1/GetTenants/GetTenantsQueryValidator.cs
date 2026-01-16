using FluentValidation;
using FSH.Modules.Multitenancy.Contracts.v1.GetTenants;

namespace FSH.Modules.Multitenancy.Features.v1.GetTenants;

public sealed class GetTenantsQueryValidator : AbstractValidator<GetTenantsQuery>
{
    public GetTenantsQueryValidator()
    {
        RuleFor(q => q.PageNumber)
            .GreaterThan(0)
            .When(q => q.PageNumber.HasValue);

        RuleFor(q => q.PageSize)
            .InclusiveBetween(1, 100)
            .When(q => q.PageSize.HasValue);

        RuleFor(q => q.Sort)
            .MaximumLength(200)
            .When(q => !string.IsNullOrEmpty(q.Sort));
    }
}
