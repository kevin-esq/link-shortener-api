namespace LinkShortener.Application.Abstractions.Services
{
    public interface IGoogleAuthService
    {
        Task<GoogleUserInfo?> ValidateGoogleTokenAsync(string idToken, CancellationToken ct);
    }

    public record GoogleUserInfo(string Email, string Name, string GoogleId, bool EmailVerified);
}
