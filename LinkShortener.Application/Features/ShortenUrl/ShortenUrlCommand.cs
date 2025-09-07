using LinkShortener.Application.DTOs;
using MediatR;

namespace LinkShortener.Application.Features.ShortenUrl
{
    namespace LinkShortener.Application.Features.ShortenUrl
    {
        public record ShortenUrlCommand(string Url, string Scheme, string Host) : IRequest<ShortenUrlResponse>;
    }
}
