using LinkShortener.Application.Abstractions;
using LinkShortener.Application.Features.Url.DTOs;
using LinkShortener.Application.Features.Url.Queries;
using MediatR;

namespace LinkShortener.Application.Features.Url.Handlers
{
    public class GetPublicUrlInfoQueryHandler : IRequestHandler<GetPublicUrlInfoQuery, GetUrlInfoResponse?>
    {
        private readonly IUrlRepository _repository;

        public GetPublicUrlInfoQueryHandler(IUrlRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetUrlInfoResponse?> Handle(GetPublicUrlInfoQuery request, CancellationToken cancellationToken)
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
