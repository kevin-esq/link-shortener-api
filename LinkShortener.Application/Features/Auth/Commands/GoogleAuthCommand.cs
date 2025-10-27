using LinkShortener.Application.Features.Auth.DTOs;
using MediatR;

namespace LinkShortener.Application.Features.Auth.Commands
{
    public record GoogleAuthCommand(string IdToken) : IRequest<LoginUserResponse>;
}
