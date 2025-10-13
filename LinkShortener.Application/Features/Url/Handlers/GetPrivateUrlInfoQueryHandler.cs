using LinkShortener.Application.Abstractions;
using LinkShortener.Application.Features.Url.DTOs;
using LinkShortener.Application.Features.Url.Queries;
using MediatR;

namespace LinkShortener.Application.Features.Url.Handlers
{
    /// <summary>
    /// Handles requests for retrieving detailed information about a shortened URL
    /// that belongs to a specific authenticated user.
    /// </summary>
    /// <remarks>
    /// This handler ensures that only the owner of the shortened URL can access its detailed information.
    /// It validates that the provided short code exists and that the <see cref="UserId"/> from the request
    /// matches the owner of the URL.
    /// </remarks>
    public class GetPrivateUrlInfoQueryHandler : IRequestHandler<GetPrivateUrlInfoQuery, GetUrlInfoResponse?>
    {
        private readonly IUrlRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetPrivateUrlInfoQueryHandler"/> class.
        /// </summary>
        /// <param name="repository">Repository abstraction for accessing URL entities.</param>
        public GetPrivateUrlInfoQueryHandler(IUrlRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Handles the query for retrieving private (authenticated) URL information.
        /// </summary>
        /// <param name="request">The query containing the short code, user ID, and request context.</param>
        /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
        /// <returns>
        /// A <see cref="GetUrlInfoResponse"/> containing information about the shortened URL,
        /// or <c>null</c> if the URL does not exist or does not belong to the user.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the provided short code is null or empty.
        /// </exception>
        public async Task<GetUrlInfoResponse?> Handle(GetPrivateUrlInfoQuery request, CancellationToken cancellationToken)
        {
            // Validate short code
            if (string.IsNullOrWhiteSpace(request.Code))
                throw new ArgumentException("Code cannot be null or empty.", nameof(request.Code));

            // Retrieve URL entity by short code
            var entity = await _repository.GetByCodeAsync(request.Code, cancellationToken);

            // Return null if URL does not exist or does not belong to this user
            if (entity is null || entity.UserId != request.UserId)
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
