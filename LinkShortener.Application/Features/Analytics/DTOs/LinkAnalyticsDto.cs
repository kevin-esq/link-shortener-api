namespace LinkShortener.Application.Features.Analytics.DTOs
{
    /// <summary>
    /// Comprehensive analytics data for a specific link.
    /// Part of the LinkPulse analytics system.
    /// </summary>
    public record LinkAnalyticsDto(
        Guid LinkId,
        string Code,
        string LongUrl,
        int TotalClicks,
        DateTime? LastAccessedOn,
        List<ClicksByDateDto> ClicksByDate,
        List<CountryStatsDto> ClicksByCountry,
        List<DeviceStatsDto> ClicksByDevice,
        List<BrowserStatsDto> ClicksByBrowser,
        List<RefererStatsDto> TopReferers
    );

    public record ClicksByDateDto(DateTime Date, int Clicks);

    public record CountryStatsDto(string Country, int Clicks, double Percentage);

    public record DeviceStatsDto(string DeviceType, int Clicks, double Percentage);

    public record BrowserStatsDto(string Browser, int Clicks, double Percentage);

    public record RefererStatsDto(string Referer, int Clicks);
}
