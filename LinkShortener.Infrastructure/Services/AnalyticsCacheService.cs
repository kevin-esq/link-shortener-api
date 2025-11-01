using LinkShortener.Application.Abstractions;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace LinkShortener.Infrastructure.Services
{
    /// <summary>
    /// Redis-based caching service for analytics data.
    /// Improves performance by reducing database load and enables event buffering.
    /// </summary>
    public class AnalyticsCacheService : IAnalyticsCacheService
    {
        private readonly IConnectionMultiplexer? _redis;
        private readonly ILogger<AnalyticsCacheService> _logger;
        private readonly bool _isRedisAvailable;

        public AnalyticsCacheService(
            IConnectionMultiplexer? redis,
            ILogger<AnalyticsCacheService> logger)
        {
            _redis = redis;
            _logger = logger;
            _isRedisAvailable = redis?.IsConnected ?? false;

            if (!_isRedisAvailable)
            {
                _logger.LogWarning("Redis is not configured or unavailable. Caching will be disabled.");
            }
        }

        public async Task CacheClickEventAsync(Guid linkId, string eventData, CancellationToken cancellationToken = default)
        {
            if (!_isRedisAvailable || _redis == null) return;

            try
            {
                var db = _redis.GetDatabase();
                var listKey = $"analytics:events:{linkId}";
                await db.ListRightPushAsync(listKey, eventData);

                // Set expiration for the list (1 hour)
                await db.KeyExpireAsync(listKey, TimeSpan.FromHours(1));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cache click event for link {LinkId}", linkId);
            }
        }

        public async Task<T?> GetAnalyticsAsync<T>(string cacheKey, CancellationToken cancellationToken = default)
        {
            if (!_isRedisAvailable || _redis == null) return default;

            try
            {
                var db = _redis.GetDatabase();
                var value = await db.StringGetAsync(cacheKey);

                if (value.IsNullOrEmpty)
                    return default;

                return JsonSerializer.Deserialize<T>(value!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get analytics from cache for key {CacheKey}", cacheKey);
                return default;
            }
        }

        public async Task SetAnalyticsAsync<T>(string cacheKey, T data, TimeSpan expiration, CancellationToken cancellationToken = default)
        {
            if (!_isRedisAvailable || _redis == null) return;

            try
            {
                var db = _redis.GetDatabase();
                var json = JsonSerializer.Serialize(data);
                await db.StringSetAsync(cacheKey, json, expiration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to set analytics in cache for key {CacheKey}", cacheKey);
            }
        }

        public async Task IncrementCounterAsync(string counterKey, CancellationToken cancellationToken = default)
        {
            if (!_isRedisAvailable || _redis == null) return;

            try
            {
                var db = _redis.GetDatabase();
                await db.StringIncrementAsync(counterKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to increment counter for key {CounterKey}", counterKey);
            }
        }
    }
}
