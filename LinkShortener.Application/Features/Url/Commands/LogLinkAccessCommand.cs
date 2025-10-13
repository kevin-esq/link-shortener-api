using MediatR;

namespace LinkShortener.Application.Features.Url.Commands
{
    public record LogLinkAccessCommand(Guid LinkId, Guid? UserId, string IpAddress, string UserAgent)
        : IRequest<Unit>;
}
