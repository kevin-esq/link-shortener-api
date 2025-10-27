using LinkShortener.Domain.Entities;

namespace LinkShortener.Application.Abstractions
{
    public interface ISessionRepository
    {
        Task<Session?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<List<Session>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<List<Session>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task AddAsync(Session session, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
        Task EndAllUserSessionsAsync(Guid userId, CancellationToken cancellationToken);
        Task<int> GetActiveSessionCountAsync(Guid userId, CancellationToken cancellationToken);
    }
}
