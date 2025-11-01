using LiteBus.Commands.Abstractions;

namespace LinkShortener.Application.Features.Auth.Commands
{
    public record VerifyForgotPasswordCodeCommand(string Email, string Code) : ICommand<bool>;
}
