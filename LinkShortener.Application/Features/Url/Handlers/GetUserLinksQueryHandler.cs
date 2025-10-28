using LinkShortener.Application.Abstractions;
using LinkShortener.Application.Features.Url.Queries;
using MediatR;

namespace LinkShortener.Application.Features.Url.Handlers
{
    public class GetUserLinksQueryHandler : IRequestHandler<GetUserLinksQuery, UserLinksResponse>
    {
        private readonly IUrlRepository _repository;

        public GetUserLinksQueryHandler(IUrlRepository repository)
        {
            _repository = repository;
        }

        public async Task<UserLinksResponse> Handle(GetUserLinksQuery request, CancellationToken cancellationToken)
        {
            var (links, totalCount) = await _repository.GetUserLinksPagedAsync(
                request.UserId,
                request.Page,
                request.PageSize,
                request.Search,
                request.OrderBy,
                request.OrderDirection,
                cancellationToken);

            var linkDtos = links.Select(l => new LinkSummaryDto(
                l.Id,
                l.Code,
                l.ShortUrl,
                l.LongUrl,
                l.CreatedOnUtc,
                l.AccessCount,
                l.LastAccessedOnUtc
            )).ToList();

            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            return new UserLinksResponse(
                linkDtos,
                totalCount,
                request.Page,
                request.PageSize,
                totalPages);
        }
    }
}
