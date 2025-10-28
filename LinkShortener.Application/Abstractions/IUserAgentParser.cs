namespace LinkShortener.Application.Abstractions
{
    /// <summary>
    /// Service for parsing User-Agent strings to extract device and browser information.
    /// </summary>
    public interface IUserAgentParser
    {
        /// <summary>
        /// Parses a User-Agent string and returns detailed device/browser information.
        /// </summary>
        UserAgentInfo Parse(string userAgent);
    }

    public record UserAgentInfo(
        string? Browser,
        string? BrowserVersion,
        string? OperatingSystem,
        string? DeviceType,
        string? DeviceBrand
    );
}
