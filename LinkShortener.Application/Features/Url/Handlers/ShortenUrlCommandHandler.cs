using LinkShortener.Application.Abstractions;
using LinkShortener.Application.Features.Url.Commands;
using LinkShortener.Application.Features.Url.DTOs;
using LinkShortener.Domain.Entities;
using LiteBus.Commands.Abstractions;

namespace LinkShortener.Application.Features.Url.Handlers
{
    public class ShortenUrlCommandHandler : ICommandHandler<ShortenUrlCommand, ShortenUrlResponse>
    {
        private readonly IUrlRepository _repository;

        public ShortenUrlCommandHandler(IUrlRepository repository)
        {
            _repository = repository;
        }

        public async Task<ShortenUrlResponse> HandleAsync(ShortenUrlCommand request, CancellationToken cancellationToken)
        {
            if (!Uri.TryCreate(request.OriginalUrl, UriKind.Absolute, out _))
                throw new ArgumentException("Invalid URL format.", nameof(request.OriginalUrl));

            string code;
            do
            {
                code = Guid.NewGuid().ToString("N")[..7].ToUpperInvariant();
            }
            while (!await _repository.IsUnique(code, cancellationToken));

            var link = Link.Create(
                longUrl: request.OriginalUrl,
                code: code,
                userId: request.UserId
            );

            link.OverrideShortUrlBase($"{request.Scheme}://{request.Host}/s/");

            await _repository.AddAsync(link, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return new ShortenUrlResponse
            {
                ShortUrl = link.ShortUrl,
                OriginalUrl = link.LongUrl,
                Code = link.Code
            };
        }
    }
}
