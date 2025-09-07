using LinkShortener.Application.DTOs.ShortenUrl;
using MediatR;

namespace LinkShortener.Application.Features.ShortenUrl
{
    namespace LinkShortener.Application.Features.ShortenUrl
    {
        public record ShortenUrlCommand(string OriginalUrl, string Scheme, string Host) : IRequest<ShortenUrlResponse>;
    }
}
