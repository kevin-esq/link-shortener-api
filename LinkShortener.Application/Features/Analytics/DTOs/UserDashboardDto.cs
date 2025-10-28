namespace LinkShortener.Application.Features.Analytics.DTOs
{
    /// <summary>
    /// Dashboard statistics for a user showing aggregated analytics across all their links.
    /// </summary>
    public record UserDashboardDto(
        int TotalLinks,
        int TotalClicks,
        int ClicksLast24Hours,
        int ClicksLast7Days,
        int ClicksLast30Days,
        List<TopLinkDto> TopLinks,
        List<ClicksByDateDto> ClicksTrend,
        List<CountryStatsDto> TopCountries,
        List<DeviceStatsDto> DeviceBreakdown
    );

    public record TopLinkDto(
        Guid LinkId,
        string Code,
        string LongUrl,
        int TotalClicks,
        DateTime? LastAccessedOn
    );
}
