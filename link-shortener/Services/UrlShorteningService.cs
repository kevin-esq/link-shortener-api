using link_shortener.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace link_shortener.Services
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

        private string GenerateCode()
        {
            var codeChars = new char[NumberOfCharsInShortlink];

            for (int i = 0; i < NumberOfCharsInShortlink; i++)
            {
                var randomIndex = _random.Next(Alphabet.Length - 1);
                codeChars[i] = Alphabet[randomIndex];
            }

            return new string(codeChars);
        }

        private async Task<bool> IsUnique(string code)
        {
            return await _cache.GetOrCreateAsync(code, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(5);
                var shortenedUrl = await _context.ShortenedUrls.FirstOrDefaultAsync(s => s.Code == code);
                return shortenedUrl is null;
            });
        }
    }
}
