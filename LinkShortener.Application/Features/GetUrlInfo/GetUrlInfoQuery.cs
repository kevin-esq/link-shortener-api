using LinkShortener.Application.DTOs.GetUrlInfo;
using MediatR;

namespace LinkShortener.Application.Features.GetUrlInfo
{
    public record GetUrlInfoQuery(string Code, string Scheme, string Host) : IRequest<GetUrlInfoResponse?>;
}