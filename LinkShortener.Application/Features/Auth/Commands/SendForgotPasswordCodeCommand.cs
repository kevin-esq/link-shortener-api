using LiteBus.Commands.Abstractions;

namespace LinkShortener.Application.Features.Auth.Commands
{
    public record SendForgotPasswordCodeCommand(string Email) : ICommand;
}
