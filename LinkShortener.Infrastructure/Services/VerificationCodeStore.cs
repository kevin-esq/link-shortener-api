using LinkShortener.Application.Abstractions.Services;
using Microsoft.Extensions.Caching.Memory;

namespace LinkShortener.Infrastructure.Services
{
    /// <summary>
    /// Provides temporary storage for email verification or password reset codes.
    /// </summary>
    public class VerificationCodeStore : IVerificationCodeStore
    {
        private readonly IMemoryCache _cache;

        /// <summary>
        /// Initializes a new instance of <see cref="VerificationCodeStore"/>.
        /// </summary>
        public VerificationCodeStore(IMemoryCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// Saves a code for a specific email with a time-to-live.
        /// </summary>
        public Task SaveCodeAsync(string email, string code, TimeSpan ttl, CancellationToken ct)
        {
            _cache.Set(GetKey(email), code, ttl);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Retrieves the code associated with an email, if available.
        /// </summary>
        public Task<string?> GetCodeAsync(string email, CancellationToken ct)
        {
            _cache.TryGetValue(GetKey(email), out string? code);
            return Task.FromResult(code);
        }

        /// <summary>
        /// Deletes a code from storage.
        /// </summary>
        public Task DeleteCodeAsync(string email, CancellationToken ct)
        {
            _cache.Remove(GetKey(email));
            return Task.CompletedTask;
        }

        private static string GetKey(string email) => $"verification-code:{email.ToLowerInvariant()}";
    }
}
