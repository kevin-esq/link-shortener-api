using LinkShortener.Domain.Entities;

namespace LinkShortener.Application.Abstractions
{
    public interface IUrlRepository
    {
        Task<ShortenedUrl?> GetByCodeAsync(string code, CancellationToken cancellationToken);
        Task AddAsync(ShortenedUrl url, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
        Task<bool> IsUnique(string code, CancellationToken cancellationToken);
    }
}