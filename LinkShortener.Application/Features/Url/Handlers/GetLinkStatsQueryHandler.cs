using LinkShortener.Application.Abstractions;
using LinkShortener.Application.Features.Url.Queries;
using LiteBus.Queries.Abstractions;

namespace LinkShortener.Application.Features.Url.Handlers
{
    public class GetLinkStatsQueryHandler : IQueryHandler<GetLinkStatsQuery, LinkStatsResponse?>
    {
        private readonly ILinkStatsRepository _repository;

        public GetLinkStatsQueryHandler(ILinkStatsRepository repository)
        {
            _repository = repository;
        }

        public async Task<LinkStatsResponse?> HandleAsync(GetLinkStatsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetLinkStatsAsync(request.Code, request.UserId, request.Days, cancellationToken);
        }
    }
}
