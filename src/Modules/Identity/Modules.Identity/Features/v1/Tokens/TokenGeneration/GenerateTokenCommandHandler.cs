using FSH.Modules.Identity.Contracts.DTOs;
using FSH.Modules.Identity.Contracts.Services;
using FSH.Modules.Identity.Contracts.v1.Tokens.TokenGeneration;
using Mediator;

namespace FSH.Modules.Identity.Features.v1.Tokens.TokenGeneration;

public sealed class GenerateTokenCommandHandler(IIdentityService identityService, ITokenService tokenService)
    : ICommandHandler<GenerateTokenCommand, TokenResponse>
{

    public async ValueTask<TokenResponse> Handle(GenerateTokenCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var identityResult = await identityService.ValidateCredentialsAsync(request.Email, request.Password, null, cancellationToken);

        if (identityResult is null)
            throw new UnauthorizedAccessException("Invalid credentials.");

        var (subject, claims) = identityResult.Value;

        return await tokenService.IssueAsync(subject, claims, null, cancellationToken);
    }
}