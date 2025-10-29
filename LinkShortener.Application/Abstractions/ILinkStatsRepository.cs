using LinkShortener.Application.Features.Url.Queries;

namespace LinkShortener.Application.Abstractions
{
    public interface ILinkStatsRepository
    {
        Task<LinkStatsResponse?> GetLinkStatsAsync(string code, Guid userId, int days, CancellationToken cancellationToken);
    }
}
