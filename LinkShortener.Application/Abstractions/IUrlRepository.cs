using LinkShortener.Domain.Entities;

namespace LinkShortener.Application.Abstractions
{
    public interface IUrlRepository
    {
        Task<Link?> GetByCodeAsync(string code, CancellationToken cancellationToken);
        Task<Link?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task AddAsync(Link link, CancellationToken cancellationToken);
        Task AddAccessAsync(LinkAccess access, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
        Task<bool> IsUnique(string code, CancellationToken cancellationToken);
    }
}
