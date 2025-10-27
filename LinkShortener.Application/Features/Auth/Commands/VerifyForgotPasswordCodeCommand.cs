using MediatR;

namespace LinkShortener.Application.Features.Auth.Commands
{
    public record VerifyForgotPasswordCodeCommand(string Email, string Code) : IRequest<bool>;
}
