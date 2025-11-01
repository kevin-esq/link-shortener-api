using LinkShortener.Application.Features.Url.DTOs;
using LiteBus.Queries.Abstractions;

namespace LinkShortener.Application.Features.Url.Queries
{
    public record GetPrivateUrlInfoQuery(string Code, string Scheme, string Host, Guid UserId) : IQuery<GetUrlInfoResponse?>;
}
