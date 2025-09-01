using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using LinkShortener.Entities;

namespace LinkShortener.Infrastructure.Services
{
    public class UrlShorteningService
    {
        public const int NumberOfCharsInShortlink = 7;
        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        private readonly Random _random = new();
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        public UrlShorteningService(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }
        public async Task<string> GenerateUniqueCode()
        {
            while (true)
            {
                var code = GenerateCode();

                if (await IsUnique(code))
                {
                    return code;
                }
            }
        }
        public async Task<string?> GetLongUrlAsync(string code)
        {
            var shortened = await _cache.GetOrCreateAsync(code, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);

                return await _context.ShortenedUrls
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Code == code);
            });

            return shortened?.LongUrl;
        }

        private string GenerateCode()
        {
            var codeChars = new char[NumberOfCharsInShortlink];

            for (int i = 0; i < NumberOfCharsInShortlink; i++)
            {
                var randomIndex = _random.Next(Alphabet.Length);
                codeChars[i] = Alphabet[randomIndex];
            }

            return new string(codeChars);
        }
        private async Task<bool> IsUnique(string code)
        {
            return !await _context.ShortenedUrls.AnyAsync(s => s.Code == code);
        }
    }
}
