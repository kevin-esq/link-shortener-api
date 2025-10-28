using LinkShortener.Application.Abstractions;
using LinkShortener.Application.Features.Url.Commands;
using MediatR;

namespace LinkShortener.Application.Features.Url.Handlers
{
    public class DeleteLinkCommandHandler : IRequestHandler<DeleteLinkCommand, bool>
    {
        private readonly IUrlRepository _repository;

        public DeleteLinkCommandHandler(IUrlRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteLinkCommand request, CancellationToken cancellationToken)
        {
            return await _repository.DeleteLinkAsync(request.Code, request.UserId, cancellationToken);
        }
    }
}
