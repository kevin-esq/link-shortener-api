using LinkShortener.Application.Abstractions;
using LinkShortener.Application.Features.Analytics.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LinkShortener.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for analytics queries.
    /// Optimized for performance with indexed queries and aggregations.
    /// </summary>
    public class AnalyticsRepository : IAnalyticsRepository
    {
        private readonly ApplicationDbContext _context;

        public AnalyticsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<LinkAnalyticsDto?> GetLinkAnalyticsAsync(Guid linkId, int daysToAnalyze, CancellationToken cancellationToken)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysToAnalyze);

            var link = await _context.Links
                .Where(l => l.Id == linkId)
                .Select(l => new { l.Id, l.Code, l.LongUrl })
                .FirstOrDefaultAsync(cancellationToken);

            if (link == null)
                return null;

            var accesses = await _context.LinkAccesses
                .Where(a => a.LinkId == linkId && a.AccessedOnUtc >= cutoffDate)
                .ToListAsync(cancellationToken);

            var totalClicks = accesses.Count;
            var lastAccessedOn = accesses.Any() ? accesses.Max(a => a.AccessedOnUtc) : (DateTime?)null;

            // Clicks by date
            var clicksByDate = accesses
                .GroupBy(a => a.AccessedOnUtc.Date)
                .Select(g => new ClicksByDateDto(g.Key, g.Count()))
                .OrderBy(x => x.Date)
                .ToList();

            // Clicks by country
            var clicksByCountry = accesses
                .Where(a => !string.IsNullOrEmpty(a.Country))
                .GroupBy(a => a.Country!)
                .Select(g => new CountryStatsDto(
                    g.Key,
                    g.Count(),
                    totalClicks > 0 ? Math.Round((double)g.Count() / totalClicks * 100, 2) : 0
                ))
                .OrderByDescending(x => x.Clicks)
                .Take(10)
                .ToList();

            // Clicks by device
            var clicksByDevice = accesses
                .Where(a => !string.IsNullOrEmpty(a.DeviceType))
                .GroupBy(a => a.DeviceType!)
                .Select(g => new DeviceStatsDto(
                    g.Key,
                    g.Count(),
                    totalClicks > 0 ? Math.Round((double)g.Count() / totalClicks * 100, 2) : 0
                ))
                .OrderByDescending(x => x.Clicks)
                .ToList();

            // Clicks by browser
            var clicksByBrowser = accesses
                .Where(a => !string.IsNullOrEmpty(a.Browser))
                .GroupBy(a => a.Browser!)
                .Select(g => new BrowserStatsDto(
                    g.Key,
                    g.Count(),
                    totalClicks > 0 ? Math.Round((double)g.Count() / totalClicks * 100, 2) : 0
                ))
                .OrderByDescending(x => x.Clicks)
                .Take(10)
                .ToList();

            // Top referers
            var topReferers = accesses
                .Where(a => !string.IsNullOrEmpty(a.Referer))
                .GroupBy(a => a.Referer!)
                .Select(g => new RefererStatsDto(g.Key, g.Count()))
                .OrderByDescending(x => x.Clicks)
                .Take(10)
                .ToList();

            return new LinkAnalyticsDto(
                link.Id,
                link.Code,
                link.LongUrl,
                totalClicks,
                lastAccessedOn,
                clicksByDate,
                clicksByCountry,
                clicksByDevice,
                clicksByBrowser,
                topReferers
            );
        }

        public async Task<UserDashboardDto> GetUserDashboardAsync(Guid userId, int daysToAnalyze, CancellationToken cancellationToken)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysToAnalyze);

            // Get user's links
            var userLinks = await _context.Links
                .Where(l => l.UserId == userId)
                .Select(l => l.Id)
                .ToListAsync(cancellationToken);

            var totalLinks = userLinks.Count;

            // Get all accesses for user's links
            var accesses = await _context.LinkAccesses
                .Where(a => userLinks.Contains(a.LinkId) && a.AccessedOnUtc >= cutoffDate)
                .ToListAsync(cancellationToken);

            var totalClicks = accesses.Count;

            // Time-based metrics
            var now = DateTime.UtcNow;
            var clicksLast24Hours = accesses.Count(a => a.AccessedOnUtc >= now.AddHours(-24));
            var clicksLast7Days = accesses.Count(a => a.AccessedOnUtc >= now.AddDays(-7));
            var clicksLast30Days = accesses.Count(a => a.AccessedOnUtc >= now.AddDays(-30));

            var linkStats = accesses
                .GroupBy(a => a.LinkId)
                .Select(g => new
                {
                    LinkId = g.Key,
                    TotalClicks = g.Count(),
                    LastAccess = g.Max(a => a.AccessedOnUtc)
                })
                .OrderByDescending(x => x.TotalClicks)
                .Take(10)
                .ToList();

            var topLinkIds = linkStats.Select(s => s.LinkId).ToList();
            var linkDetails = await _context.Links
                .Where(l => topLinkIds.Contains(l.Id))
                .Select(l => new { l.Id, l.Code, l.LongUrl })
                .ToListAsync(cancellationToken);

            var topLinks = linkStats
                .Join(linkDetails,
                    stats => stats.LinkId,
                    link => link.Id,
                    (stats, link) => new TopLinkDto(
                        link.Id,
                        link.Code,
                        link.LongUrl,
                        stats.TotalClicks,
                        stats.LastAccess
                    )
                )
                .ToList();

            // Clicks trend
            var clicksTrend = accesses
                .GroupBy(a => a.AccessedOnUtc.Date)
                .Select(g => new ClicksByDateDto(g.Key, g.Count()))
                .OrderBy(x => x.Date)
                .ToList();

            // Top countries
            var topCountries = accesses
                .Where(a => !string.IsNullOrEmpty(a.Country))
                .GroupBy(a => a.Country!)
                .Select(g => new CountryStatsDto(
                    g.Key,
                    g.Count(),
                    totalClicks > 0 ? Math.Round((double)g.Count() / totalClicks * 100, 2) : 0
                ))
                .OrderByDescending(x => x.Clicks)
                .Take(10)
                .ToList();

            // Device breakdown
            var deviceBreakdown = accesses
                .Where(a => !string.IsNullOrEmpty(a.DeviceType))
                .GroupBy(a => a.DeviceType!)
                .Select(g => new DeviceStatsDto(
                    g.Key,
                    g.Count(),
                    totalClicks > 0 ? Math.Round((double)g.Count() / totalClicks * 100, 2) : 0
                ))
                .OrderByDescending(x => x.Clicks)
                .ToList();

            return new UserDashboardDto(
                totalLinks,
                totalClicks,
                clicksLast24Hours,
                clicksLast7Days,
                clicksLast30Days,
                topLinks,
                clicksTrend,
                topCountries,
                deviceBreakdown
            );
        }
    }
}
