using LinkShortener.Application.Features.Auth.DTOs;
using MediatR;

namespace LinkShortener.Application.Features.Auth.Commands
{
    public record RegisterUserCommand(string Username, string Email, string Password)
            : IRequest<RegisterUserResponse>;
}
