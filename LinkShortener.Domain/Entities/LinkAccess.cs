using LinkShortener.Domain.Common;

namespace LinkShortener.Domain.Entities
{
    /// <summary>
    /// Represents a detailed access/click event for a shortened link.
    /// Part of the LinkPulse analytics system.
    /// </summary>
    public class LinkAccess : BaseEntity
    {
        private LinkAccess() { }

        public LinkAccess(
            Guid linkId,
            Guid? userId,
            string ipAddress,
            string userAgent,
            string? referer = null,
            string? country = null,
            string? city = null,
            string? browser = null,
            string? browserVersion = null,
            string? operatingSystem = null,
            string? deviceType = null,
            string? deviceBrand = null)
        {
            Id = Guid.NewGuid();
            LinkId = linkId;
            UserId = userId;
            AccessedOnUtc = DateTime.UtcNow;
            IpAddress = ipAddress;
            UserAgent = userAgent;
            Referer = referer;
            Country = country;
            City = city;
            Browser = browser;
            BrowserVersion = browserVersion;
            OperatingSystem = operatingSystem;
            DeviceType = deviceType;
            DeviceBrand = deviceBrand;
        }

        public Guid LinkId { get; private set; }
        public Link Link { get; private set; } = null!;

        public Guid? UserId { get; private set; }
        public User? User { get; private set; }

        public DateTime AccessedOnUtc { get; private set; }

        // Basic tracking
        public string IpAddress { get; private set; } = string.Empty;
        public string UserAgent { get; private set; } = string.Empty;
        public string? Referer { get; private set; }

        // Geographic data
        public string? Country { get; private set; }
        public string? City { get; private set; }

        // Browser & Device analytics
        public string? Browser { get; private set; }
        public string? BrowserVersion { get; private set; }
        public string? OperatingSystem { get; private set; }
        public string? DeviceType { get; private set; } // Mobile, Desktop, Tablet, Bot
        public string? DeviceBrand { get; private set; } // Apple, Samsung, etc.
    }
}
