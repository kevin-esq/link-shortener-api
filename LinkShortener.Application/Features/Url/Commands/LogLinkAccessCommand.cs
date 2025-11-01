using LiteBus.Commands.Abstractions;

namespace LinkShortener.Application.Features.Url.Commands
{
    public record LogLinkAccessCommand(Guid LinkId, Guid? UserId, string IpAddress, string UserAgent)
        : ICommand;
}
