using Google.Apis.Auth;
using LinkShortener.Application.Abstractions.Services;
using Microsoft.Extensions.Logging;

namespace LinkShortener.Infrastructure.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly string _clientId;
        private readonly ILogger<GoogleAuthService> _logger;

        public GoogleAuthService(string clientId, ILogger<GoogleAuthService> logger)
        {
            _clientId = clientId;
            _logger = logger;
        }

        public async Task<GoogleUserInfo?> ValidateGoogleTokenAsync(string idToken, CancellationToken ct)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _clientId }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

                return new GoogleUserInfo(
                    payload.Email,
                    payload.Name,
                    payload.Subject,
                    payload.EmailVerified
                );
            }
            catch (InvalidJwtException ex)
            {
                _logger.LogWarning(ex, "Invalid Google token");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating Google token");
                throw;
            }
        }
    }
}
