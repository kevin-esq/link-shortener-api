using LinkShortener.Application.Features.Url.DTOs;
using MediatR;


namespace LinkShortener.Application.Features.Url.Commands
{
    public record ShortenUrlCommand(string OriginalUrl, string Scheme, string Host, Guid UserId) : IRequest<ShortenUrlResponse>;
}