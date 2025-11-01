using LiteBus.Commands.Abstractions;

namespace LinkShortener.Application.Features.Url.Commands
{
    public record DeleteLinkCommand(
        string Code,
        Guid UserId) : ICommand<bool>;
}
