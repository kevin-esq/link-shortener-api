using LinkShortener.Application.Features.Auth.DTOs;
using MediatR;

namespace LinkShortener.Application.Features.Auth.Commands
{
    public record LoginUserCommand(string Email, string Password)
        : IRequest<LoginUserResponse>;
}
