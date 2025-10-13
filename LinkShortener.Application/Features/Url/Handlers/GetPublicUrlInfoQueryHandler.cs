using LinkShortener.Application.Abstractions;
using LinkShortener.Application.Features.Url.DTOs;
using LinkShortener.Application.Features.Url.Queries;
using MediatR;

namespace LinkShortener.Application.Features.Url.Handlers
{
    /// <summary>
    /// Handles requests for retrieving public information about a shortened URL.
    /// </summary>
    /// <remarks>
    /// This query handler allows anyone (authenticated or not) to obtain general information
    /// about a shortened URL based on its short code.
    /// Unlike <see cref="GetPrivateUrlInfoQueryHandler"/>, it does not validate ownership.
    /// </remarks>
    public class GetPublicUrlInfoQueryHandler : IRequestHandler<GetPublicUrlInfoQuery, GetUrlInfoResponse?>
    {
        private readonly IUrlRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetPublicUrlInfoQueryHandler"/> class.
        /// </summary>
        /// <param name="repository">Repository abstraction for accessing URL entities.</param>
        public GetPublicUrlInfoQueryHandler(IUrlRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Handles the query for retrieving public URL information.
        /// </summary>
        /// <param name="request">The query containing the short code and request context (scheme and host).</param>
        /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
        /// <returns>
        /// A <see cref="GetUrlInfoResponse"/> containing information about the shortened URL,
        /// or <c>null</c> if the short code does not exist.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the provided short code is null or empty.
        /// </exception>
        public async Task<GetUrlInfoResponse?> Handle(GetPublicUrlInfoQuery request, CancellationToken cancellationToken)
        {
            // Validate short code
            if (string.IsNullOrWhiteSpace(request.Code))
                throw new ArgumentException("Code cannot be null or empty.", nameof(request.Code));

            // Retrieve URL entity by short code
            var entity = await _repository.GetByCodeAsync(request.Code, cancellationToken);

            // Return null if the short code does not exist
            if (entity is null)
                return null;

            // Construct the full short URL using the current scheme and host
            entity.OverrideShortUrlBase($"{request.Scheme}://{request.Host}/s/");

            // Return the response DTO
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
