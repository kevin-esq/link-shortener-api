using MediatR;

namespace LinkShortener.Application.Features.Url.Queries
{
    public record GetQrCodeQuery(
        string Code,
        Guid UserId,
        int Size = 300,
        string Format = "png") : IRequest<QrCodeResponse?>;

    public record QrCodeResponse(
        string Code,
        string ShortUrl,
        byte[] QrCodeImage,
        string Format,
        int Size);
}
