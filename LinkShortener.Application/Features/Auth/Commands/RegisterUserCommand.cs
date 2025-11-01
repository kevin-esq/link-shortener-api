using LinkShortener.Application.Features.Auth.DTOs;
using LiteBus.Commands.Abstractions;

namespace LinkShortener.Application.Features.Auth.Commands
{
    public record RegisterUserCommand(string Username, string Email, string Password)
            : ICommand<RegisterUserResponse>;
}
