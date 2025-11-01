using LiteBus.Commands.Abstractions;

namespace LinkShortener.Application.Features.Auth.Commands
{
    public record VerifyEmailCodeCommand(string Email, string Code) : ICommand<bool>;
}
