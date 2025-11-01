using LinkShortener.Application.Abstractions;
using LinkShortener.Application.Features.Url.DTOs;
using LinkShortener.Application.Features.Url.Queries;
using LiteBus.Queries.Abstractions;

namespace LinkShortener.Application.Features.Url.Handlers
{
    public class GetPublicUrlInfoQueryHandler : IQueryHandler<GetPublicUrlInfoQuery, GetUrlInfoResponse?>
    {
        private readonly IUrlRepository _repository;

        public GetPublicUrlInfoQueryHandler(IUrlRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetUrlInfoResponse?> HandleAsync(GetPublicUrlInfoQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Code))
                throw new ArgumentException("Code cannot be null or empty.", nameof(request.Code));

            var entity = await _repository.GetByCodeAsync(request.Code, cancellationToken);
            if (entity is null)
                return null;

            entity.OverrideShortUrlBase($"{request.Scheme}://{request.Host}/s/");

            return new GetUrlInfoResponse
            {
                Id = entity.Id,
                ShortUrl = entity.ShortUrl,
                OriginalUrl = entity.LongUrl,
                CreatedAt = entity.CreatedOnUtc
            };
        }
    }
}
