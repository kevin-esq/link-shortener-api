namespace LinkShortener.Application.Abstractions
{
    /// <summary>
    /// Service for caching analytics data to improve performance and reduce database load.
    /// </summary>
    public interface IAnalyticsCacheService
    {
        /// <summary>
        /// Caches click event for later batch processing.
        /// </summary>
        Task CacheClickEventAsync(Guid linkId, string eventData, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves cached analytics data for a specific link.
        /// </summary>
        Task<T?> GetAnalyticsAsync<T>(string cacheKey, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets analytics data in cache with expiration.
        /// </summary>
        Task SetAnalyticsAsync<T>(string cacheKey, T data, TimeSpan expiration, CancellationToken cancellationToken = default);

        /// <summary>
        /// Increments a counter in cache (e.g., total clicks for a link).
        /// </summary>
        Task IncrementCounterAsync(string counterKey, CancellationToken cancellationToken = default);
    }
}
