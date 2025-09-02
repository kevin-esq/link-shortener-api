
using LinkShortener.Application.DTOs;

namespace LinkShortener.Application.Abstractions
{
    public interface IUrlShorteningAppService
    {
        Task<ShortenUrlResponse> ShortenUrlAsync(string url, string scheme, string host);
        Task<string?> GetLongUrlAsync(string code);
    }
}
