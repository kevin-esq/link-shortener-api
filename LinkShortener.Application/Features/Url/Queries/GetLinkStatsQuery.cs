using MediatR;

namespace LinkShortener.Application.Features.Url.Queries
{
    public record GetLinkStatsQuery(
        string Code,
        Guid UserId,
        int Days) : IRequest<LinkStatsResponse?>;

    public record LinkStatsResponse(
        string Code,
        string LongUrl,
        int TotalClicks,
        int UniqueVisitors,
        DateTime? LastAccessed,
        List<DailyClicksDto> ClicksByDay,
        List<CountryClicksDto> ClicksByCountry,
        List<DeviceClicksDto> ClicksByDevice,
        List<BrowserClicksDto> ClicksByBrowser,
        List<RefererClicksDto> TopReferers);

    public record DailyClicksDto(DateTime Date, int Clicks);
    public record CountryClicksDto(string Country, int Clicks, double Percentage);
    public record DeviceClicksDto(string Device, int Clicks, double Percentage);
    public record BrowserClicksDto(string Browser, int Clicks, double Percentage);
    public record RefererClicksDto(string Referer, int Clicks);
}
