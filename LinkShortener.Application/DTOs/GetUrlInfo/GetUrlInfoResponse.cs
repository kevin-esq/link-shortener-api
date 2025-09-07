namespace LinkShortener.Application.DTOs.GetUrlInfo
{
    public class GetUrlInfoResponse
    {
        public string ShortUrl { get; set; } = string.Empty;
        public string OriginalUrl { get; set; } = string.Empty;
        //public int? Clicks { get; set; } // TODO: Implement Clicks tracking
        public DateTime CreatedAt { get; set; }
    }
}
