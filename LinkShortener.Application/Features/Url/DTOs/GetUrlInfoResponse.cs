namespace LinkShortener.Application.Features.Url.DTOs
{
    public class GetUrlInfoResponse
    {
        public Guid Id { get; set; }
        public string ShortUrl { get; set; } = string.Empty;
        public string OriginalUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
