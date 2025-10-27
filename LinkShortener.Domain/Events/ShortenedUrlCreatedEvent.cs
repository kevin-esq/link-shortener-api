using LinkShortener.Domain.Common;

namespace LinkShortener.Domain.Events
{
    public class ShortenedUrlCreatedEvent(Guid shortenedUrlId, Guid userId, string code) : IDomainEvent
    {
        public Guid ShortenedUrlId { get; } = shortenedUrlId;
        public Guid UserId { get; } = userId;
        public string Code { get; } = code;
        public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
    }
}
