using LinkShortener.Application.Features.Analytics.DTOs;

namespace LinkShortener.Application.Abstractions
{
    /// <summary>
    /// Repository for analytics queries on link access data.
    /// Provides optimized queries for dashboard and reporting.
    /// </summary>
    public interface IAnalyticsRepository
    {
        Task<LinkAnalyticsDto?> GetLinkAnalyticsAsync(Guid linkId, int daysToAnalyze, CancellationToken cancellationToken);
        Task<UserDashboardDto> GetUserDashboardAsync(Guid userId, int daysToAnalyze, CancellationToken cancellationToken);
    }
}
