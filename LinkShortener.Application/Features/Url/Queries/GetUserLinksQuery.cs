using LinkShortener.Application.Features.Url.DTOs;
using MediatR;

namespace LinkShortener.Application.Features.Url.Queries
{
    public record GetUserLinksQuery(
        Guid UserId,
        int Page,
        int PageSize,
        string? Search = null,
        string? OrderBy = "createdAt",
        string? OrderDirection = "desc") : IRequest<UserLinksResponse>;

    public record UserLinksResponse(
        List<LinkSummaryDto> Links,
        int TotalCount,
        int Page,
        int PageSize,
        int TotalPages);

    public record LinkSummaryDto(
        Guid Id,
        string Code,
        string ShortUrl,
        string LongUrl,
        DateTime CreatedAt,
        int TotalClicks,
        DateTime? LastAccessed);
}
