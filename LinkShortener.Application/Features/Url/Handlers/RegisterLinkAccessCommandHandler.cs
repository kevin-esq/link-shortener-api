using LinkShortener.Application.Abstractions;
using LinkShortener.Application.Features.Url.Commands;
using LinkShortener.Domain.Entities;
using MediatR;

namespace LinkShortener.Application.Features.Url.Handlers
{
    public class RegisterLinkAccessCommandHandler : IRequestHandler<RegisterLinkAccessCommand, Unit>
    {
        private readonly IUrlRepository _repository;

        public RegisterLinkAccessCommandHandler(IUrlRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(RegisterLinkAccessCommand request, CancellationToken cancellationToken)
        {
            var link = await _repository.GetByIdAsync(request.LinkId, cancellationToken);
            if (link == null)
                throw new InvalidOperationException($"No existe un enlace con el ID {request.LinkId}");

            var access = new LinkAccess(
                linkId: request.LinkId,
                userId: request.UserId,
                ipAddress: request.IpAddress,
                userAgent: request.UserAgent
            );

            await _repository.AddAccessAsync(access, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
