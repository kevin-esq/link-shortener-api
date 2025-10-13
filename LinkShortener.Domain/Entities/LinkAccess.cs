using LinkShortener.Domain.Common;

namespace LinkShortener.Domain.Entities
{
    public class LinkAccess : BaseEntity
    {
        private LinkAccess() { }

        public LinkAccess(Guid linkId, Guid? userId, string ipAddress, string userAgent)
        {
            Id = Guid.NewGuid();
            LinkId = linkId;
            UserId = userId;
            AccessedOnUtc = DateTime.UtcNow;
            IpAddress = ipAddress;
            UserAgent = userAgent;
        }

        public Guid LinkId { get; private set; }
        public Link Link { get; private set; } = null!;

        public Guid? UserId { get; private set; }
        public User? User { get; private set; }

        public DateTime AccessedOnUtc { get; private set; }
        public string IpAddress { get; private set; } = string.Empty;
        public string UserAgent { get; private set; } = string.Empty;
    }
}
