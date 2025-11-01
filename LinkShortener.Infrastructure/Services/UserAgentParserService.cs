using LinkShortener.Application.Abstractions;
using UAParser;

namespace LinkShortener.Infrastructure.Services
{
    /// <summary>
    /// Service for parsing User-Agent strings using UAParser library.
    /// Extracts browser, OS, and device information from HTTP User-Agent headers.
    /// </summary>
    public class UserAgentParserService : IUserAgentParser
    {
        private readonly Parser _parser;

        public UserAgentParserService()
        {
            _parser = Parser.GetDefault();
        }

        public UserAgentInfo Parse(string userAgent)
        {
            if (string.IsNullOrWhiteSpace(userAgent))
            {
                return new UserAgentInfo(null, null, null, null, null);
            }

            try
            {
                var clientInfo = _parser.Parse(userAgent);

                var browser = clientInfo.UA.Family != "Other" ? clientInfo.UA.Family : null;
                var browserVersion = !string.IsNullOrEmpty(clientInfo.UA.Major)
                    ? $"{clientInfo.UA.Major}.{clientInfo.UA.Minor}"
                    : null;

                var os = clientInfo.OS.Family != "Other" ? clientInfo.OS.Family : null;

                var deviceType = DetermineDeviceType(clientInfo, userAgent);
                var deviceBrand = clientInfo.Device.Brand != "Other" ? clientInfo.Device.Brand : null;

                return new UserAgentInfo(browser, browserVersion, os, deviceType, deviceBrand);
            }
            catch
            {
                // If parsing fails, return nulls
                return new UserAgentInfo(null, null, null, null, null);
            }
        }

        private static string DetermineDeviceType(ClientInfo clientInfo, string userAgent)
        {
            // Check if it's a bot/crawler
            if (IsBot(userAgent))
                return "Bot";

            // Use device family if available
            if (clientInfo.Device.Family != null && clientInfo.Device.Family != "Other")
            {
                var family = clientInfo.Device.Family.ToLowerInvariant();
                if (family.Contains("tablet") || family.Contains("ipad"))
                    return "Tablet";
                if (family.Contains("mobile") || family.Contains("phone"))
                    return "Mobile";
            }

            // Check OS hints
            var os = clientInfo.OS.Family?.ToLowerInvariant() ?? "";
            if (os.Contains("android") || os.Contains("ios"))
            {
                if (userAgent.Contains("Tablet") || userAgent.Contains("iPad"))
                    return "Tablet";
                return "Mobile";
            }

            // Default to Desktop
            return "Desktop";
        }

        private static bool IsBot(string userAgent)
        {
            var botKeywords = new[] { "bot", "crawler", "spider", "scraper", "curl", "wget", "python" };
            var lowerUserAgent = userAgent.ToLowerInvariant();
            return botKeywords.Any(keyword => lowerUserAgent.Contains(keyword));
        }
    }
}
