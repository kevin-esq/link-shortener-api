using LinkShortener.Application.Features.Analytics.DTOs;
using MediatR;

namespace LinkShortener.Application.Features.Analytics.Queries
{
    /// <summary>
    /// Query to retrieve comprehensive analytics for a specific link.
    /// </summary>
    public record GetLinkAnalyticsQuery(
        Guid LinkId,
        int DaysToAnalyze = 30
    ) : IRequest<LinkAnalyticsDto?>;
}
