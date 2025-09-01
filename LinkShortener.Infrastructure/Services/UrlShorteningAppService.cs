using LinkShortener.Application.Abstractions;
using LinkShortener.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace LinkShortener.Infrastructure.Services
{
    public class UrlShorteningAppService(
        UrlShorteningService urlShorteningService,
        ApplicationDbContext dbContext,
        IMemoryCache cache) : IUrlShorteningAppService
    {
        private readonly UrlShorteningService _urlShorteningService = urlShorteningService;
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly IMemoryCache _cache = cache;

        public async Task<string> ShortenUrlAsync(string url, string scheme, string host)
        {
            var code = await _urlShorteningService.GenerateUniqueCode();

            var shortenedUrl = new ShortenedUrl
            {
                Id = Guid.NewGuid(),
                LongUrl = url,
                Code = code,
                ShortUrl = $"{scheme}://{host}/s/{code}",
                CreateOnUtc = DateTime.UtcNow
            };

            _dbContext.ShortenedUrls.Add(shortenedUrl);
            await _dbContext.SaveChangesAsync();

            return shortenedUrl.ShortUrl;
        }

        public async Task<string?> GetLongUrlAsync(string code)
        {
            var shortenedCode = await _cache.GetOrCreateAsync(code, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(5);
                return await _dbContext.ShortenedUrls.FirstOrDefaultAsync(s => s.Code == code);
            });

            return shortenedCode?.LongUrl;
        }
    }
}
