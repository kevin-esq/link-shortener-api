using LinkShortener.Domain.Common;

namespace LinkShortener.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        private RefreshToken() { }

        public static RefreshToken Create(Guid userId, string token, DateTime expiresAt)
        {
            return new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Token = token,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt,
                IsRevoked = false,
                IsUsed = false
            };
        }

        public Guid UserId { get; private set; }
        public string Token { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public bool IsRevoked { get; private set; }
        public bool IsUsed { get; private set; }
        public DateTime? RevokedAt { get; private set; }
        public DateTime? UsedAt { get; private set; }
        public string? ReplacedByToken { get; private set; }

        public User User { get; private set; } = null!;

        public bool IsValid => !IsRevoked && !IsUsed && ExpiresAt > DateTime.UtcNow;

        public void Revoke()
        {
            IsRevoked = true;
            RevokedAt = DateTime.UtcNow;
        }

        public void MarkAsUsed(string? replacedByToken = null)
        {
            IsUsed = true;
            UsedAt = DateTime.UtcNow;
            ReplacedByToken = replacedByToken;
        }
    }
}
