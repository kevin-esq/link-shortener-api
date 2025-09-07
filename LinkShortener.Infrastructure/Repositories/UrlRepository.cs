using LinkShortener.Application.Abstractions;
using LinkShortener.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace LinkShortener.Infrastructure.Repositories
{
    public class UrlRepository : IUrlRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        public UrlRepository(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<ShortenedUrl?> GetByCodeAsync(string code, CancellationToken cancellationToken)
        {
            return await _cache.GetOrCreateAsync(code, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(5);
                return await _context.ShortenedUrls
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Code == code);
            });
        }

        public async Task AddAsync(ShortenedUrl url, CancellationToken cancellationToken)
        {
            await _context.ShortenedUrls.AddAsync(url, cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task<bool> IsUnique(string code, CancellationToken cancellationToken)
        {
            return !await _context.ShortenedUrls.AnyAsync(s => s.Code == code, cancellationToken);
        }
    }
}
