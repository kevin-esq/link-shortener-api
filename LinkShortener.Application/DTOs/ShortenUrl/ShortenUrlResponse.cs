namespace LinkShortener.Application.DTOs.ShortenUrl
{
    public class ShortenUrlResponse
    {
        public string ShortUrl { get; set; } = string.Empty;
        public string OriginalUrl { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? CreatedAt { get; set; }
    }
}