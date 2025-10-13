using LinkShortener.Application.Abstractions;
using LinkShortener.Application.Features.Url.Commands;
using LinkShortener.Application.Features.Url.DTOs;
using LinkShortener.Domain.Entities;
using MediatR;

namespace LinkShortener.Application.Features.Url.Handlers
{
    /// <summary>
    /// Handles the command to create a shortened version of a given long URL.
    /// </summary>
    /// <remarks>
    /// This command handler is responsible for validating the input URL,
    /// generating a unique short code, creating the corresponding <see cref="Link"/> entity,
    /// and persisting it in the repository.
    /// </remarks>
    public class ShortenUrlCommandHandler : IRequestHandler<ShortenUrlCommand, ShortenUrlResponse>
    {
        private readonly IUrlRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShortenUrlCommandHandler"/> class.
        /// </summary>
        /// <param name="repository">Repository abstraction for accessing and persisting URL entities.</param>
        public ShortenUrlCommandHandler(IUrlRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Handles the logic for shortening a long URL.
        /// </summary>
        /// <param name="request">Command containing the original URL, request context (scheme and host), and the user ID.</param>
        /// <param name="cancellationToken">Token to cancel the asynchronous operation if needed.</param>
        /// <returns>
        /// A <see cref="ShortenUrlResponse"/> object containing the generated short URL, the original URL, and its unique code.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the provided URL is invalid or not in a valid absolute format.
        /// </exception>
        public async Task<ShortenUrlResponse> Handle(ShortenUrlCommand request, CancellationToken cancellationToken)
        {
            // Validate that the provided URL has a valid format
            if (!Uri.TryCreate(request.OriginalUrl, UriKind.Absolute, out _))
                throw new ArgumentException("Invalid URL format.", nameof(request.OriginalUrl));

            // Generate a unique 7-character code
            string code;
            do
            {
                code = Guid.NewGuid().ToString("N")[..7].ToUpperInvariant();
            }
            while (!await _repository.IsUnique(code, cancellationToken));

            // Create a new shortened link entity
            var link = Link.Create(
                longUrl: request.OriginalUrl,
                code: code,
                userId: request.UserId
            );

            // Assign the base URL
            link.OverrideShortUrlBase($"{request.Scheme}://{request.Host}/s/");

            // Persist the new link in the database
            await _repository.AddAsync(link, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            // Return the resulting DTO
            return new ShortenUrlResponse
            {
                ShortUrl = link.ShortUrl,
                OriginalUrl = link.LongUrl,
                Code = link.Code
            };
        }
    }
}
