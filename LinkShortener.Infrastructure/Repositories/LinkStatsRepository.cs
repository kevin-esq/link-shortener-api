using LinkShortener.Application.Abstractions;
using LinkShortener.Application.Features.Url.Queries;
using Microsoft.EntityFrameworkCore;

namespace LinkShortener.Infrastructure.Repositories
{
    public class LinkStatsRepository : ILinkStatsRepository
    {
        private readonly ApplicationDbContext _context;

        public LinkStatsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<LinkStatsResponse?> GetLinkStatsAsync(string code, Guid userId, int days, CancellationToken cancellationToken)
        {
            var link = await _context.Links
                .FirstOrDefaultAsync(l => l.Code == code && l.UserId == userId, cancellationToken);

            if (link == null)
                return null;

            var cutoffDate = DateTime.UtcNow.AddDays(-days);

            var clickEvents = await _context.ClickEvents
                .Where(c => c.LinkId == link.Id && c.Timestamp >= cutoffDate)
                .ToListAsync(cancellationToken);

            var totalClicks = clickEvents.Count;
            var uniqueVisitors = clickEvents.Select(c => c.IpAddress).Distinct().Count();
            var lastAccessed = clickEvents.Any() ? clickEvents.Max(c => c.Timestamp) : (DateTime?)null;

            var clicksByDay = clickEvents
                .GroupBy(c => c.Timestamp.Date)
                .Select(g => new DailyClicksDto(g.Key, g.Count()))
                .OrderBy(d => d.Date)
                .ToList();

            var clicksByCountry = clickEvents
                .Where(c => !string.IsNullOrEmpty(c.Country))
                .GroupBy(c => c.Country!)
                .Select(g => new CountryClicksDto(
                    g.Key,
                    g.Count(),
                    totalClicks > 0 ? Math.Round((double)g.Count() / totalClicks * 100, 2) : 0))
                .OrderByDescending(c => c.Clicks)
                .Take(10)
                .ToList();

            var clicksByDevice = clickEvents
                .GroupBy(c => c.DeviceType)
                .Select(g => new DeviceClicksDto(
                    g.Key,
                    g.Count(),
                    totalClicks > 0 ? Math.Round((double)g.Count() / totalClicks * 100, 2) : 0))
                .OrderByDescending(c => c.Clicks)
                .ToList();

            var clicksByBrowser = clickEvents
                .Where(c => !string.IsNullOrEmpty(c.Browser))
                .GroupBy(c => c.Browser!)
                .Select(g => new BrowserClicksDto(
                    g.Key,
                    g.Count(),
                    totalClicks > 0 ? Math.Round((double)g.Count() / totalClicks * 100, 2) : 0))
                .OrderByDescending(c => c.Clicks)
                .Take(10)
                .ToList();

            var topReferers = clickEvents
                .Where(c => !string.IsNullOrEmpty(c.Referrer))
                .GroupBy(c => c.Referrer!)
                .Select(g => new RefererClicksDto(g.Key, g.Count()))
                .OrderByDescending(r => r.Clicks)
                .Take(10)
                .ToList();

            return new LinkStatsResponse(
                link.Code,
                link.LongUrl,
                totalClicks,
                uniqueVisitors,
                lastAccessed,
                clicksByDay,
                clicksByCountry,
                clicksByDevice,
                clicksByBrowser,
                topReferers);
        }
    }
}
