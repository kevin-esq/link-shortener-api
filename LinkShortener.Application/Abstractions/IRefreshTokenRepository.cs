using LinkShortener.Domain.Entities;

namespace LinkShortener.Application.Abstractions
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken);
        Task<List<RefreshToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
        Task RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken);
    }
}
