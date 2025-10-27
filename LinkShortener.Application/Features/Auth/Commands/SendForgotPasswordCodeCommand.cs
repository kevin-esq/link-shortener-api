using MediatR;

namespace LinkShortener.Application.Features.Auth.Commands
{
    public record SendForgotPasswordCodeCommand(string Email) : IRequest;
}
