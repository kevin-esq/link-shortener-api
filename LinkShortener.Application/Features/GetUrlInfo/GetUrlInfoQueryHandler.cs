using LinkShortener.Application.Abstractions;
using LinkShortener.Application.DTOs;
using MediatR;
using System;

namespace LinkShortener.Application.Features.GetUrlInfo
{
    public class GetUrlInfoQueryHandler : IRequestHandler<GetUrlInfoQuery, ShortenUrlResponse?>
    {
        private readonly IUrlRepository _repository;

        public GetUrlInfoQueryHandler(IUrlRepository repository)
        {
            _repository = repository;
        }

        public async Task<ShortenUrlResponse?> Handle(GetUrlInfoQuery request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByCodeAsync(request.Code, cancellationToken);

            if (entity is null)
                return null;

            return new ShortenUrlResponse
            {
                ShortUrl = $"https://localhost:7092/s/{entity.Code}",
                OriginalUrl = entity.LongUrl,
                Code = entity.Code,
            };
        }
    }
}