using LinkShortener.Application.Abstractions;
using LinkShortener.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace LinkShortener.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for managing <see cref="Link"/> and <see cref="LinkAccess"/> entities.
    /// </summary>
    /// <remarks>
    /// This repository provides persistence and retrieval operations for links,
    /// including caching frequently accessed links in memory to reduce database load.
    /// </remarks>
    public class UrlRepository : IUrlRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlRepository"/> class.
        /// </summary>
        /// <param name="context">Database context for accessing persistence layer.</param>
        /// <param name="cache">In-memory cache service for performance optimization.</param>
        public UrlRepository(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        #region Links

        /// <summary>
        /// Retrieves a link entity by its unique short code.
        /// </summary>
        /// <param name="code">The short code associated with the link.</param>
        /// <param name="cancellationToken">Token for canceling the asynchronous operation.</param>
        /// <returns>
        /// A <see cref="Link"/> instance if found; otherwise, <c>null</c>.
        /// </returns>
        /// <remarks>
        /// This method first attempts to retrieve the link from the in-memory cache.
        /// If it's not found, it queries the database and stores the result in the cache
        /// for subsequent fast retrievals. The cache entry expires after 5 minutes of inactivity.
        /// </remarks>
        public async Task<Link?> GetByCodeAsync(string code, CancellationToken cancellationToken)
        {
            var cacheKey = $"Link_{code}";

            if (!_cache.TryGetValue(cacheKey, out Link? cachedLink))
            {
                cachedLink = await _context.Links
                    .Include(l => l.Accesses)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(l => l.Code == code, cancellationToken);

                if (cachedLink != null)
                {
                    var cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(5)
                    };

                    _cache.Set(cacheKey, cachedLink, cacheEntryOptions);
                }
            }

            return cachedLink;
        }

        /// <summary>
        /// Retrieves a link entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique ID of the link.</param>
        /// <param name="cancellationToken">Token for canceling the asynchronous operation.</param>
        /// <returns>The <see cref="Link"/> entity if found; otherwise, <c>null</c>.</returns>
        public async Task<Link?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Links
                .Include(l => l.Accesses)
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        }

        /// <summary>
        /// Adds a new link entity to the database context.
        /// </summary>
        /// <param name="link">The link entity to add.</param>
        /// <param name="cancellationToken">Token for canceling the asynchronous operation.</param>
        public async Task AddAsync(Link link, CancellationToken cancellationToken)
        {
            await _context.Links.AddAsync(link, cancellationToken);
        }

        /// <summary>
        /// Persists all pending changes to the database.
        /// </summary>
        /// <param name="cancellationToken">Token for canceling the asynchronous operation.</param>
        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Checks whether the provided short code is unique in the database.
        /// </summary>
        /// <param name="code">The short code to verify.</param>
        /// <param name="cancellationToken">Token for canceling the asynchronous operation.</param>
        /// <returns><c>true</c> if the code is unique; otherwise, <c>false</c>.</returns>
        public async Task<bool> IsUnique(string code, CancellationToken cancellationToken)
        {
            return !await _context.Links.AnyAsync(l => l.Code == code, cancellationToken);
        }

        #endregion

        #region LinkAccess (Auditoría)

        /// <summary>
        /// Registers a new access event for a given link.
        /// </summary>
        /// <param name="access">The <see cref="LinkAccess"/> entity containing access metadata (IP, UserAgent, etc.).</param>
        /// <param name="cancellationToken">Token for canceling the asynchronous operation.</param>
        /// <remarks>
        /// This method is used to track link usage analytics such as the number of clicks,
        /// source IP address, and user agent.
        /// </remarks>
        public async Task AddAccessAsync(LinkAccess access, CancellationToken cancellationToken)
        {
            await _context.LinkAccesses.AddAsync(access, cancellationToken);
        }

        #endregion
    }
}
