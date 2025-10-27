using LinkShortener.Domain.Common;

namespace LinkShortener.Domain.Entities
{
    public class Session : BaseEntity
    {
        private Session() { }

        public static Session Create(
            Guid userId,
            string ipAddress,
            string userAgent,
            string? deviceName = null,
            string? location = null)
        {
            return new Session
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                DeviceName = deviceName,
                Location = location,
                CreatedAt = DateTime.UtcNow,
                LastActivityAt = DateTime.UtcNow,
                IsActive = true
            };
        }

        public Guid UserId { get; private set; }
        public string IpAddress { get; private set; } = string.Empty;
        public string UserAgent { get; private set; } = string.Empty;
        public string? DeviceName { get; private set; }
        public string? Location { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime LastActivityAt { get; private set; }
        public DateTime? EndedAt { get; private set; }
        public bool IsActive { get; private set; }
        public string? RefreshTokenId { get; private set; }

        public User User { get; private set; } = null!;

        public void UpdateActivity()
        {
            LastActivityAt = DateTime.UtcNow;
        }

        public void End()
        {
            IsActive = false;
            EndedAt = DateTime.UtcNow;
        }

        public void SetRefreshToken(string refreshTokenId)
        {
            RefreshTokenId = refreshTokenId;
        }

        public TimeSpan Duration => EndedAt.HasValue 
            ? EndedAt.Value - CreatedAt 
            : DateTime.UtcNow - CreatedAt;
    }
}
