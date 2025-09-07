namespace LinkShortener.Application.DTOs
{
    public class ShortenUrlResponse
    {
        public string ShortUrl { get; set; } = String.Empty;
        public string OriginalUrl { get; set; } = String.Empty;
        public string Code { get; set; } = String.Empty;
        public string? CreatedAt { get; set; } = String.Empty;
    }
}