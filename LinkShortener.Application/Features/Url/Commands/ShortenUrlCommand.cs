using LinkShortener.Application.Features.Url.DTOs;
using LiteBus.Commands.Abstractions;

namespace LinkShortener.Application.Features.Url.Commands
{
    public record ShortenUrlCommand(string OriginalUrl, string Scheme, string Host, Guid UserId) : ICommand<ShortenUrlResponse>;
}