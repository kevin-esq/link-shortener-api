using LinkShortener.Application.Abstractions;
using LinkShortener.Application.Features.Url.Queries;
using MediatR;

namespace LinkShortener.Application.Features.Url.Handlers
{
    public class GetQrCodeQueryHandler : IRequestHandler<GetQrCodeQuery, QrCodeResponse?>
    {
        private readonly IUrlRepository _repository;
        private readonly IQrCodeService _qrCodeService;

        public GetQrCodeQueryHandler(IUrlRepository repository, IQrCodeService qrCodeService)
        {
            _repository = repository;
            _qrCodeService = qrCodeService;
        }

        public async Task<QrCodeResponse?> Handle(GetQrCodeQuery request, CancellationToken cancellationToken)
        {
            var link = await _repository.GetByCodeAsync(request.Code, cancellationToken);

            if (link == null || link.UserId != request.UserId)
                return null;

            var qrCodeImage = _qrCodeService.GenerateQrCode(link.ShortUrl, request.Size);

            return new QrCodeResponse(
                link.Code,
                link.ShortUrl,
                qrCodeImage,
                request.Format,
                request.Size);
        }
    }
}
