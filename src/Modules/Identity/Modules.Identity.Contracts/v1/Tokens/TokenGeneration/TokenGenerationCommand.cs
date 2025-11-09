using Mediator;

namespace FSH.Modules.Identity.Contracts.v1.Tokens.TokenGeneration;

public record TokenGenerationCommand(
    string Email,
    string Password)
    : ICommand<TokenGenerationCommandResponse>;