using LinkShortener.Application.Abstractions;
using LinkShortener.Application.Features.Url.DTOs;
using LinkShortener.Application.Features.Url.Queries;
using MediatR;

namespace LinkShortener.Application.Features.Url.Handlers
{
    public class GetPrivateUrlInfoQueryHandler : IRequestHandler<GetPrivateUrlInfoQuery, GetUrlInfoResponse?>
    {
        private readonly IUrlRepository _repository;

        public GetPrivateUrlInfoQueryHandler(IUrlRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetUrlInfoResponse?> Handle(GetPrivateUrlInfoQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Code))
                throw new ArgumentException("Code cannot be null or empty.", nameof(request.Code));

            var entity = await _repository.GetByCodeAsync(request.Code, cancellationToken);
            if (entity is null || entity.UserId != request.UserId)
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
