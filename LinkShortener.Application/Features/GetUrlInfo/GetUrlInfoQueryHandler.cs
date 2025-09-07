using LinkShortener.Application.Abstractions;
using LinkShortener.Application.DTOs.GetUrlInfo;
using LinkShortener.Application.DTOs.ShortenUrl;
using MediatR;
using System;

namespace LinkShortener.Application.Features.GetUrlInfo
{
    public class GetUrlInfoQueryHandler(IUrlRepository repository) : IRequestHandler<GetUrlInfoQuery, GetUrlInfoResponse?>
    {
        private readonly IUrlRepository _repository = repository;

        public async Task<GetUrlInfoResponse?> Handle(GetUrlInfoQuery request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByCodeAsync(request.Code, cancellationToken);

            if (entity is null)
                return null;

            return new GetUrlInfoResponse
            {
                ShortUrl = $"{request.Scheme}://{request.Host}/s/{request.Code}",
                OriginalUrl = entity.LongUrl,
                CreatedAt = entity.CreateOnUtc
            };
        }
    }
}