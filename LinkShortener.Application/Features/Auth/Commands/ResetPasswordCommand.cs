using MediatR;

namespace LinkShortener.Application.Features.Auth.Commands
{
    public record ResetPasswordCommand(string Email, string Code, string NewPassword) : IRequest;
}
