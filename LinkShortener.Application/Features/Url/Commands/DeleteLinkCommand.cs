using MediatR;

namespace LinkShortener.Application.Features.Url.Commands
{
    public record DeleteLinkCommand(
        string Code,
        Guid UserId) : IRequest<bool>;
}
