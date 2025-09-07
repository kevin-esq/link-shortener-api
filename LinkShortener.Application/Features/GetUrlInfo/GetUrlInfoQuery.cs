using LinkShortener.Application.DTOs;
using MediatR;

namespace LinkShortener.Application.Features.GetUrlInfo
{
    public record GetUrlInfoQuery(string Code) : IRequest<ShortenUrlResponse?>;
}
