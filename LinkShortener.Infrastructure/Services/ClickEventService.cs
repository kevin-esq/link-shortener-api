using LinkShortener.Application.Abstractions;
using LinkShortener.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LinkShortener.Infrastructure.Services
{
    public class ClickEventService : IClickEventService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserAgentParser _uaParser;
        private readonly IGeolocationService _geoService;
        private readonly ILogger<ClickEventService> _logger;

        public ClickEventService(
            ApplicationDbContext context,
            IUserAgentParser uaParser,
            IGeolocationService geoService,
            ILogger<ClickEventService> logger)
        {
            _context = context;
            _uaParser = uaParser;
            _geoService = geoService;
            _logger = logger;
        }

        public async Task RecordClickAsync(
            Guid linkId,
            string shortCode,
            string destination,
            Guid? userId,
            string? referrer,
            Dictionary<string, string>? utmParams,
            string ipAddress,
            string userAgent,
            string? acceptLanguage,
            string status,
            int latencyMs,
            string? redirectType = null,
            string? requestId = null,
            string? traceId = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var uaInfo = _uaParser.Parse(userAgent);
                var geoInfo = await _geoService.GetLocationAsync(ipAddress, cancellationToken);

                var clickEvent = ClickEvent.Create(
                    linkId,
                    shortCode,
                    destination,
                    userId,
                    Truncate(referrer, 2048),
                    Truncate(utmParams?.GetValueOrDefault("utm_source"), 100),
                    Truncate(utmParams?.GetValueOrDefault("utm_medium"), 100),
                    Truncate(utmParams?.GetValueOrDefault("utm_campaign"), 100),
                    Truncate(utmParams?.GetValueOrDefault("utm_content"), 100),
                    Truncate(utmParams?.GetValueOrDefault("utm_term"), 100),
                    ipAddress,
                    Truncate(geoInfo?.Country, 100),
                    null,
                    Truncate(geoInfo?.City, 100),
                    geoInfo?.Latitude,
                    geoInfo?.Longitude,
                    Truncate(uaInfo?.DeviceType ?? DeviceTypes.Unknown, 20),
                    Truncate(uaInfo?.OperatingSystem, 50),
                    null,
                    Truncate(uaInfo?.Browser, 50),
                    Truncate(uaInfo?.BrowserVersion, 20),
                    Truncate(userAgent, 512),
                    Truncate(acceptLanguage, 50),
                    status,
                    latencyMs,
                    false,
                    false,
                    redirectType,
                    requestId,
                    traceId
                );

                _context.ClickEvents.Add(clickEvent);
                await _context.SaveChangesAsync(cancellationToken);
                
                _logger.LogInformation("Click event recorded: LinkId={LinkId}, Code={Code}", linkId, shortCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to record click event for LinkId={LinkId}, Code={Code}", linkId, shortCode);
            }
        }

        public async Task<ClickEventStatsDto> GetStatsAsync(
            Guid linkId,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.ClickEvents.Where(c => c.LinkId == linkId);

            if (fromDate.HasValue)
                query = query.Where(c => c.Timestamp >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(c => c.Timestamp <= toDate.Value);

            var events = await query.ToListAsync(cancellationToken);

            var totalClicks = events.Count;
            var uniqueVisitors = events.Select(e => e.IpAddress).Distinct().Count();
            var blockedAttempts = events.Count(e => e.Status == ClickStatus.Blocked);
            var avgLatencyMs = events.Any() ? events.Average(e => e.LatencyMs) : 0;

            var clicksByCountry = events
                .Where(e => !string.IsNullOrEmpty(e.Country))
                .GroupBy(e => e.Country!)
                .ToDictionary(g => g.Key, g => g.Count());

            var clicksByDevice = events
                .GroupBy(e => e.DeviceType)
                .ToDictionary(g => g.Key, g => g.Count());

            var clicksByHour = events
                .GroupBy(e => e.Timestamp.Hour)
                .ToDictionary(g => g.Key.ToString(), g => g.Count());

            return new ClickEventStatsDto(
                totalClicks,
                uniqueVisitors,
                blockedAttempts,
                avgLatencyMs,
                clicksByCountry,
                clicksByDevice,
                clicksByHour
            );
        }

        public async Task<List<ClickEventDto>> GetRecentClicksAsync(
            Guid linkId,
            int limit = 100,
            CancellationToken cancellationToken = default)
        {
            var clicks = await _context.ClickEvents
                .Where(c => c.LinkId == linkId)
                .OrderByDescending(c => c.Timestamp)
                .Take(limit)
                .Select(c => new ClickEventDto(
                    c.Id,
                    c.Timestamp,
                    c.LinkId,
                    c.ShortCode,
                    c.UserId,
                    c.Referrer,
                    c.Country,
                    c.City,
                    c.DeviceType,
                    c.Browser,
                    c.Status,
                    c.LatencyMs
                ))
                .ToListAsync(cancellationToken);

            return clicks;
        }

        private static string? Truncate(string? value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }
}
