using LinkShortener.Application.Features.Analytics.DTOs;
using LiteBus.Queries.Abstractions;

namespace LinkShortener.Application.Features.Analytics.Queries
{
    /// <summary>
    /// Query to retrieve dashboard analytics for a user across all their links.
    /// </summary>
    public record GetUserDashboardQuery(
        Guid UserId,
        int DaysToAnalyze = 30
    ) : IQuery<UserDashboardDto>;
}
