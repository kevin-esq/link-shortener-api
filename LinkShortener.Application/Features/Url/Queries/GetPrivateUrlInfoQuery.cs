using LinkShortener.Application.Features.Url.DTOs;
using MediatR;

namespace LinkShortener.Application.Features.Url.Queries
{
    public record GetPrivateUrlInfoQuery(string Code, string Scheme, string Host, Guid UserId) : IRequest<GetUrlInfoResponse?>;
}
