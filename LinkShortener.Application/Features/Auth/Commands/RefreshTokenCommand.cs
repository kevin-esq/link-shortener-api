using LinkShortener.Application.Features.Auth.DTOs;
using LiteBus.Commands.Abstractions;

namespace LinkShortener.Application.Features.Auth.Commands
{
    public record RefreshTokenCommand(string RefreshToken) : ICommand<LoginUserResponse>;
}
