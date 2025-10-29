using LinkShortener.Domain.Common;

namespace LinkShortener.Domain.Entities
{
    public class LinkMetric : BaseEntity
    {
        private LinkMetric() { }

        public static LinkMetric Create(
            Guid linkId,
            DateTime date,
            int clicksTotal,
            int uniqueVisitors,
            int qrScans,
            int blockedAttempts,
            double avgLatencyMs,
            double errorRate,
            string? topCountry,
            string? topDevice,
            string? topBrowser,
            string? topReferrer)
        {
            return new LinkMetric
            {
                Id = Guid.NewGuid(),
                LinkId = linkId,
                Date = date,
                ClicksTotal = clicksTotal,
                UniqueVisitors = uniqueVisitors,
                QrScans = qrScans,
                BlockedAttempts = blockedAttempts,
                AvgLatencyMs = avgLatencyMs,
                ErrorRate = errorRate,
                TopCountry = topCountry,
                TopDevice = topDevice,
                TopBrowser = topBrowser,
                TopReferrer = topReferrer,
                CreatedAt = DateTime.UtcNow
            };
        }

        public Guid LinkId { get; private set; }
        public DateTime Date { get; private set; }
        public int ClicksTotal { get; private set; }
        public int UniqueVisitors { get; private set; }
        public int QrScans { get; private set; }
        public int BlockedAttempts { get; private set; }
        public double AvgLatencyMs { get; private set; }
        public double ErrorRate { get; private set; }
        public string? TopCountry { get; private set; }
        public string? TopDevice { get; private set; }
        public string? TopBrowser { get; private set; }
        public string? TopReferrer { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public void Update(
            int clicksTotal,
            int uniqueVisitors,
            int qrScans,
            int blockedAttempts,
            double avgLatencyMs,
            double errorRate)
        {
            ClicksTotal = clicksTotal;
            UniqueVisitors = uniqueVisitors;
            QrScans = qrScans;
            BlockedAttempts = blockedAttempts;
            AvgLatencyMs = avgLatencyMs;
            ErrorRate = errorRate;
        }
    }

    public class UserMetric : BaseEntity
    {
        private UserMetric() { }

        public static UserMetric Create(
            Guid userId,
            DateTime date,
            int linksCreated,
            int activeLinks,
            int totalClicks,
            int apiCalls)
        {
            return new UserMetric
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Date = date,
                LinksCreated = linksCreated,
                ActiveLinks = activeLinks,
                TotalClicks = totalClicks,
                ApiCalls = apiCalls,
                CreatedAt = DateTime.UtcNow
            };
        }

        public Guid UserId { get; private set; }
        public DateTime Date { get; private set; }
        public int LinksCreated { get; private set; }
        public int ActiveLinks { get; private set; }
        public int TotalClicks { get; private set; }
        public int ApiCalls { get; private set; }
        public DateTime CreatedAt { get; private set; }
    }
}
