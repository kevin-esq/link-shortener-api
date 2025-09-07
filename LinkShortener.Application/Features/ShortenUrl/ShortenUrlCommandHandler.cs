using LinkShortener.Application.Abstractions;
using LinkShortener.Application.DTOs.ShortenUrl;
using LinkShortener.Application.Features.ShortenUrl.LinkShortener.Application.Features.ShortenUrl;
using LinkShortener.Domain.Entities;
using MediatR;

namespace LinkShortener.Application.Features.ShortenUrl
{
    public class ShortenUrlCommandHandler(IUrlRepository repository) : IRequestHandler<ShortenUrlCommand, ShortenUrlResponse>
    {
        private readonly IUrlRepository _repository = repository;

        public async Task<ShortenUrlResponse> Handle(ShortenUrlCommand request, CancellationToken cancellationToken)
        {
            if (!Uri.TryCreate(request.OriginalUrl, UriKind.Absolute, out _))
                throw new ArgumentException("Invalid URL");

            string code;

            do
            {
                code = Guid.NewGuid().ToString("N")[..7].ToUpper();
            }
            while (!await _repository.IsUnique(code, cancellationToken));

            var entity = new ShortenedUrl
            {
                Id = Guid.NewGuid(),
                LongUrl = request.OriginalUrl,
                Code = code,
                ShortUrl = $"{request.Scheme}://{request.Host}/s/{code}",
                CreateOnUtc = DateTime.UtcNow
            };

            await _repository.AddAsync(entity, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return new ShortenUrlResponse
            {
                ShortUrl = entity.ShortUrl,
                OriginalUrl = entity.LongUrl,
                Code = entity.Code,
            };
        }
    }
}
