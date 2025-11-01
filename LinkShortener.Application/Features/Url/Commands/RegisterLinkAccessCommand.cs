using LiteBus.Commands.Abstractions;

namespace LinkShortener.Application.Features.Url.Commands
{
    public record RegisterLinkAccessCommand(
        Guid LinkId,
        string IpAddress,
        string UserAgent,
        Guid? UserId = null,
        string? Referer = null
    ) : ICommand;
}
