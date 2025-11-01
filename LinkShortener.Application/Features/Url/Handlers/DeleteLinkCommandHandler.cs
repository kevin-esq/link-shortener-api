using LinkShortener.Application.Abstractions;
using LinkShortener.Application.Features.Url.Commands;
using LiteBus.Commands.Abstractions;

namespace LinkShortener.Application.Features.Url.Handlers
{
    public class DeleteLinkCommandHandler : ICommandHandler<DeleteLinkCommand, bool>
    {
        private readonly IUrlRepository _repository;

        public DeleteLinkCommandHandler(IUrlRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> HandleAsync(DeleteLinkCommand request, CancellationToken cancellationToken)
        {
            return await _repository.DeleteLinkAsync(request.Code, request.UserId, cancellationToken);
        }
    }
}
