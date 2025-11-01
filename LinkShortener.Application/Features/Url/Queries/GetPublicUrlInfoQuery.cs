using LinkShortener.Application.Features.Url.DTOs;
using LiteBus.Queries.Abstractions;

namespace LinkShortener.Application.Features.Url.Queries
{
    public record GetPublicUrlInfoQuery(string Code, string Scheme, string Host) : IQuery<GetUrlInfoResponse?>;
}
