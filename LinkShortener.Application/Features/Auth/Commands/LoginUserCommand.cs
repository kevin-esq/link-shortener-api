using LinkShortener.Application.Features.Auth.DTOs;
using LiteBus.Commands.Abstractions;

namespace LinkShortener.Application.Features.Auth.Commands
{
    public record LoginUserCommand(string Email, string Password)
        : ICommand<LoginUserResponse>;
}
