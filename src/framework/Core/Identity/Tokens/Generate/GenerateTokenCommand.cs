using Mediator;

namespace FSH.Framework.Core.Identity.Tokens.Generate;
public sealed record GenerateTokenCommand(
    string Email,
    string Password
) : ICommand<TokenResponse>;