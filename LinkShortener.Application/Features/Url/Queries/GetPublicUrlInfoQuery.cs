using LinkShortener.Application.Features.Url.DTOs;
using MediatR;

namespace LinkShortener.Application.Features.Url.Queries
{
    public record GetPublicUrlInfoQuery(string Code, string Scheme, string Host) : IRequest<GetUrlInfoResponse?>;
}
