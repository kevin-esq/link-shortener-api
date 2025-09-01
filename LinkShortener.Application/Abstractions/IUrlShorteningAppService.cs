
namespace LinkShortener.Application.Abstractions
{
    public interface IUrlShorteningAppService
    {
        Task<string> ShortenUrlAsync(string url, string scheme, string host);
        Task<string?> GetLongUrlAsync(string code);
    }
}
