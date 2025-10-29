using LinkShortener.Application.Abstractions;
using Microsoft.Extensions.Logging;
using System.Net;

namespace LinkShortener.Infrastructure.Services
{
    /// <summary>
    /// Service for IP geolocation.
    /// Currently provides basic implementation. Can be enhanced with external APIs like:
    /// - IP2Location
    /// - MaxMind GeoIP2
    /// - ipapi.co
    /// - ipgeolocation.io
    /// </summary>
    public class GeolocationService : IGeolocationService
    {
        private readonly ILogger<GeolocationService> _logger;

        public GeolocationService(ILogger<GeolocationService> logger)
        {
            _logger = logger;
        }

        public Task<GeolocationInfo> GetLocationAsync(string ipAddress, CancellationToken cancellationToken = default)
        {
            // Basic implementation - returns Unknown for now
            // TODO: Integrate with external geolocation API
            
            if (string.IsNullOrWhiteSpace(ipAddress) || ipAddress == "unknown")
            {
                return Task.FromResult(new GeolocationInfo("Unknown", "Unknown"));
            }

            // Check if it's a local/private IP
            if (IsPrivateOrLocalIp(ipAddress))
            {
                return Task.FromResult(new GeolocationInfo("Local", "Local"));
            }

            // For production, you would call an external API here
            // Example with ipapi.co (requires HttpClient):
            /*
            try
            {
                var response = await _httpClient.GetFromJsonAsync<IpapiResponse>(
                    $"https://ipapi.co/{ipAddress}/json/", 
                    cancellationToken);
                    
                return new GeolocationInfo(
                    response?.Country, 
                    response?.City,
                    response?.CountryCode,
                    response?.Latitude,
                    response?.Longitude
                );
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get geolocation for IP {IpAddress}", ipAddress);
                return new GeolocationInfo("Unknown", "Unknown");
            }
            */

            // Temporary placeholder
            return Task.FromResult(new GeolocationInfo("Unknown", "Unknown"));
        }

        private static bool IsPrivateOrLocalIp(string ipAddress)
        {
            if (!IPAddress.TryParse(ipAddress, out var ip))
                return false;

            // Check for localhost
            if (IPAddress.IsLoopback(ip))
                return true;

            // Check for private ranges
            var bytes = ip.GetAddressBytes();
            if (bytes.Length == 4) // IPv4
            {
                // 10.0.0.0/8
                if (bytes[0] == 10)
                    return true;

                // 172.16.0.0/12
                if (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31)
                    return true;

                // 192.168.0.0/16
                if (bytes[0] == 192 && bytes[1] == 168)
                    return true;
            }

            return false;
        }
    }
}
