using LinkShortener.Application.Features.Auth.DTOs;
using MediatR;

namespace LinkShortener.Application.Features.Auth.Commands
{
    public record RefreshTokenCommand(string RefreshToken) : IRequest<LoginUserResponse>;
}
