using LinkShortener.Application.Abstractions;
using LinkShortener.Application.Features.Url.Commands;
using LinkShortener.Domain.Entities;
using MediatR;

namespace LinkShortener.Application.Features.Url.Handlers
{
    /// <summary>
    /// Handles the registration of an access event for a shortened URL.
    /// </summary>
    /// <remarks>
    /// This command handler is responsible for recording when a user or visitor
    /// accesses a shortened link. It stores metadata such as the user's IP address
    /// and user agent for analytical purposes.
    /// </remarks>
    public class RegisterLinkAccessCommandHandler : IRequestHandler<RegisterLinkAccessCommand, Unit>
    {
        private readonly IUrlRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterLinkAccessCommandHandler"/> class.
        /// </summary>
        /// <param name="repository">Repository abstraction for accessing and persisting URL entities and related data.</param>
        public RegisterLinkAccessCommandHandler(IUrlRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Handles the command to register a link access event.
        /// </summary>
        /// <param name="request">Command containing the link access details (link ID, user, IP address, user agent).</param>
        /// <param name="cancellationToken">Token to cancel the asynchronous operation if needed.</param>
        /// <returns>A <see cref="Unit"/> value indicating completion of the command.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the provided <paramref name="request.LinkId"/> does not correspond to an existing link.
        /// </exception>
        public async Task<Unit> Handle(RegisterLinkAccessCommand request, CancellationToken cancellationToken)
        {
            // Retrieve the link entity by ID
            var link = await _repository.GetByIdAsync(request.LinkId, cancellationToken);
            if (link == null)
                throw new InvalidOperationException($"No existe un enlace con el ID {request.LinkId}");

            // Create a new access record
            var access = new LinkAccess(
                linkId: request.LinkId,
                userId: request.UserId,
                ipAddress: request.IpAddress,
                userAgent: request.UserAgent
            );

            // Persist the new access event
            await _repository.AddAccessAsync(access, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
