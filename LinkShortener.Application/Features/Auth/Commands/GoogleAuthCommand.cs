using LinkShortener.Application.Features.Auth.DTOs;
using LiteBus.Commands.Abstractions;

namespace LinkShortener.Application.Features.Auth.Commands
{
    public record GoogleAuthCommand(string IdToken) : ICommand<LoginUserResponse>;
}
