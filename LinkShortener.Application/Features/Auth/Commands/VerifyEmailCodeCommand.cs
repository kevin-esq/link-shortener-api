using MediatR;

namespace LinkShortener.Application.Features.Auth.Commands
{
    public record VerifyEmailCodeCommand(string Email, string Code) : IRequest<bool>;
}
