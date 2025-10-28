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
        Task<(List<LinkWithStats> links, int totalCount)> GetUserLinksPagedAsync(
            Guid userId, 
            int page, 
            int pageSize,
            string? search,
            string? orderBy,
            string? orderDirection,
            CancellationToken cancellationToken);
        Task<bool> DeleteLinkAsync(string code, Guid userId, CancellationToken cancellationToken);
    }

    public record LinkWithStats(
        Guid Id,
        string Code,
        string ShortUrl,
        string LongUrl,
        DateTime CreatedOnUtc,
        int AccessCount,
        DateTime? LastAccessedOnUtc);
}
