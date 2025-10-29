namespace LinkShortener.Application.Abstractions
{
    /// <summary>
    /// Service for retrieving geographic information from IP addresses.
    /// </summary>
    public interface IGeolocationService
    {
        /// <summary>
        /// Gets geographic information for the given IP address.
        /// </summary>
        Task<GeolocationInfo> GetLocationAsync(string ipAddress, CancellationToken cancellationToken = default);
    }

    public record GeolocationInfo(
        string? Country,
        string? City,
        string? CountryCode = null,
        double? Latitude = null,
        double? Longitude = null
    );
}
