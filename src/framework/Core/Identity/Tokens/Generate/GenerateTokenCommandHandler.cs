using Mediator;

namespace FSH.Framework.Core.Identity.Tokens.Generate;
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