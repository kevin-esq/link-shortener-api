namespace LinkShortener.Application.Abstractions
{
    public interface IClickEventService
    {
        Task RecordClickAsync(
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
            CancellationToken cancellationToken = default);

        Task<ClickEventStatsDto> GetStatsAsync(
            Guid linkId,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            CancellationToken cancellationToken = default);

        Task<List<ClickEventDto>> GetRecentClicksAsync(
            Guid linkId,
            int limit = 100,
            CancellationToken cancellationToken = default);
    }

    public record ClickEventDto(
        Guid Id,
        DateTime Timestamp,
        Guid LinkId,
        string ShortCode,
        Guid? UserId,
        string? Referrer,
        string? Country,
        string? City,
        string DeviceType,
        string? Browser,
        string Status,
        int LatencyMs);

    public record ClickEventStatsDto(
        int TotalClicks,
        int UniqueVisitors,
        int BlockedAttempts,
        double AvgLatencyMs,
        Dictionary<string, int> ClicksByCountry,
        Dictionary<string, int> ClicksByDevice,
        Dictionary<string, int> ClicksByHour);
}
