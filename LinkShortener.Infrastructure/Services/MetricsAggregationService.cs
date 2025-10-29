using LinkShortener.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LinkShortener.Infrastructure.Services
{
    public class MetricsAggregationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MetricsAggregationService> _logger;
        private readonly TimeSpan _aggregationInterval = TimeSpan.FromHours(1);

        public MetricsAggregationService(
            IServiceProvider serviceProvider,
            ILogger<MetricsAggregationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Metrics Aggregation Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await AggregateMetricsAsync(stoppingToken);
                    await Task.Delay(_aggregationInterval, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error aggregating metrics");
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
        }

        private async Task AggregateMetricsAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var yesterday = DateTime.UtcNow.Date.AddDays(-1);

            await AggregateLinkMetricsAsync(context, yesterday, cancellationToken);
            await AggregateUserMetricsAsync(context, yesterday, cancellationToken);

            _logger.LogInformation("Metrics aggregated for {Date}", yesterday);
        }

        private async Task AggregateLinkMetricsAsync(
            ApplicationDbContext context,
            DateTime date,
            CancellationToken cancellationToken)
        {
            var links = await context.Links.Select(l => l.Id).ToListAsync(cancellationToken);

            foreach (var linkId in links)
            {
                var events = await context.ClickEvents
                    .Where(e => e.LinkId == linkId && e.Timestamp.Date == date)
                    .ToListAsync(cancellationToken);

                if (!events.Any())
                    continue;

                var metric = await context.LinkMetrics
                    .FirstOrDefaultAsync(m => m.LinkId == linkId && m.Date == date, cancellationToken);

                var clicksTotal = events.Count;
                var uniqueVisitors = events.Select(e => e.IpAddress).Distinct().Count();
                var qrScans = 0;
                var blockedAttempts = events.Count(e => e.Status == ClickStatus.Blocked);
                var avgLatencyMs = events.Average(e => e.LatencyMs);
                var errorRate = 0.0;

                var topCountry = events
                    .Where(e => !string.IsNullOrEmpty(e.Country))
                    .GroupBy(e => e.Country)
                    .OrderByDescending(g => g.Count())
                    .FirstOrDefault()?.Key;

                var topDevice = events
                    .GroupBy(e => e.DeviceType)
                    .OrderByDescending(g => g.Count())
                    .FirstOrDefault()?.Key;

                var topBrowser = events
                    .Where(e => !string.IsNullOrEmpty(e.Browser))
                    .GroupBy(e => e.Browser)
                    .OrderByDescending(g => g.Count())
                    .FirstOrDefault()?.Key;

                var topReferrer = events
                    .Where(e => !string.IsNullOrEmpty(e.Referrer))
                    .GroupBy(e => e.Referrer)
                    .OrderByDescending(g => g.Count())
                    .FirstOrDefault()?.Key;

                if (metric == null)
                {
                    metric = LinkMetric.Create(
                        linkId,
                        date,
                        clicksTotal,
                        uniqueVisitors,
                        qrScans,
                        blockedAttempts,
                        avgLatencyMs,
                        errorRate,
                        topCountry,
                        topDevice,
                        topBrowser,
                        topReferrer
                    );
                    context.LinkMetrics.Add(metric);
                }
                else
                {
                    metric.Update(clicksTotal, uniqueVisitors, qrScans, blockedAttempts, avgLatencyMs, errorRate);
                }
            }

            await context.SaveChangesAsync(cancellationToken);
        }

        private async Task AggregateUserMetricsAsync(
            ApplicationDbContext context,
            DateTime date,
            CancellationToken cancellationToken)
        {
            var users = await context.Users.Select(u => u.Id).ToListAsync(cancellationToken);

            foreach (var userId in users)
            {
                var linksCreated = await context.Links
                    .CountAsync(l => l.UserId == userId && l.CreatedOnUtc.Date == date, cancellationToken);

                var activeLinks = await context.Links
                    .CountAsync(l => l.UserId == userId, cancellationToken);

                var userLinkIds = await context.Links
                    .Where(l => l.UserId == userId)
                    .Select(l => l.Id)
                    .ToListAsync(cancellationToken);

                var totalClicks = await context.ClickEvents
                    .CountAsync(e => userLinkIds.Contains(e.LinkId) && e.Timestamp.Date == date, cancellationToken);

                var metric = await context.UserMetrics
                    .FirstOrDefaultAsync(m => m.UserId == userId && m.Date == date, cancellationToken);

                if (metric == null)
                {
                    metric = UserMetric.Create(userId, date, linksCreated, activeLinks, totalClicks, 0);
                    context.UserMetrics.Add(metric);
                }
            }

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
