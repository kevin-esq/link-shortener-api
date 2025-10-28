using LinkShortener.Domain.Common;

namespace LinkShortener.Domain.Entities
{
    public class ClickEvent : BaseEntity
    {
        private ClickEvent() { }

        public static ClickEvent Create(
            Guid linkId,
            string shortCode,
            string destination,
            Guid? userId,
            string? referrer,
            string? utmSource,
            string? utmMedium,
            string? utmCampaign,
            string? utmContent,
            string? utmTerm,
            string ipAddress,
            string? country,
            string? region,
            string? city,
            double? latitude,
            double? longitude,
            string deviceType,
            string? os,
            string? osVersion,
            string? browser,
            string? browserVersion,
            string userAgent,
            string? acceptLanguage,
            string status,
            int latencyMs,
            bool phishingDetected,
            bool malwareDetected,
            string? redirectType,
            string? requestId,
            string? traceId)
        {
            return new ClickEvent
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow,
                LinkId = linkId,
                ShortCode = shortCode,
                Destination = destination,
                UserId = userId,
                Referrer = referrer,
                UtmSource = utmSource,
                UtmMedium = utmMedium,
                UtmCampaign = utmCampaign,
                UtmContent = utmContent,
                UtmTerm = utmTerm,
                IpAddress = ipAddress,
                Country = country,
                Region = region,
                City = city,
                Latitude = latitude,
                Longitude = longitude,
                DeviceType = deviceType,
                Os = os,
                OsVersion = osVersion,
                Browser = browser,
                BrowserVersion = browserVersion,
                UserAgent = userAgent,
                AcceptLanguage = acceptLanguage,
                Status = status,
                LatencyMs = latencyMs,
                PhishingDetected = phishingDetected,
                MalwareDetected = malwareDetected,
                RedirectType = redirectType,
                RequestId = requestId,
                TraceId = traceId
            };
        }

        public DateTime Timestamp { get; private set; }
        public Guid LinkId { get; private set; }
        public string ShortCode { get; private set; } = string.Empty;
        public string Destination { get; private set; } = string.Empty;
        public Guid? UserId { get; private set; }
        public string? Referrer { get; private set; }
        public string? UtmSource { get; private set; }
        public string? UtmMedium { get; private set; }
        public string? UtmCampaign { get; private set; }
        public string? UtmContent { get; private set; }
        public string? UtmTerm { get; private set; }
        public string IpAddress { get; private set; } = string.Empty;
        public string? Country { get; private set; }
        public string? Region { get; private set; }
        public string? City { get; private set; }
        public double? Latitude { get; private set; }
        public double? Longitude { get; private set; }
        public string DeviceType { get; private set; } = string.Empty;
        public string? Os { get; private set; }
        public string? OsVersion { get; private set; }
        public string? Browser { get; private set; }
        public string? BrowserVersion { get; private set; }
        public string UserAgent { get; private set; } = string.Empty;
        public string? AcceptLanguage { get; private set; }
        public string Status { get; private set; } = string.Empty;
        public int LatencyMs { get; private set; }
        public bool PhishingDetected { get; private set; }
        public bool MalwareDetected { get; private set; }
        public string? RedirectType { get; private set; }
        public string? RequestId { get; private set; }
        public string? TraceId { get; private set; }
    }

    public static class ClickStatus
    {
        public const string Redirected = "redirected";
        public const string Blocked = "blocked";
        public const string PreviewShown = "preview_shown";
        public const string Expired = "expired";
        public const string PasswordRequired = "password_required";
    }

    public static class DeviceTypes
    {
        public const string Mobile = "mobile";
        public const string Desktop = "desktop";
        public const string Tablet = "tablet";
        public const string Bot = "bot";
        public const string Unknown = "unknown";
    }
}
