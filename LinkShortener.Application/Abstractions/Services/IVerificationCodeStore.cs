namespace LinkShortener.Application.Abstractions.Services
{
    public interface IVerificationCodeStore
    {
        Task SaveCodeAsync(string email, string code, TimeSpan ttl, CancellationToken ct);
        Task<string?> GetCodeAsync(string email, CancellationToken ct);
        Task DeleteCodeAsync(string email, CancellationToken ct);
    }
}
