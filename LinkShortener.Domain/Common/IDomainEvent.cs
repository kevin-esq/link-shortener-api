namespace LinkShortener.Domain.Common
{
    public interface IDomainEvent
    {
        DateTime OccurredOnUtc { get; }
    }
}
