using LinkShortener.Application.Abstractions;
using QRCoder;

namespace LinkShortener.Infrastructure.Services
{
    public class QrCodeService : IQrCodeService
    {
        public byte[] GenerateQrCode(string url, int size = 300)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            
            return qrCode.GetGraphic(size / 25);
        }

        public string GenerateQrCodeBase64(string url, int size = 300)
        {
            var qrCodeBytes = GenerateQrCode(url, size);
            return Convert.ToBase64String(qrCodeBytes);
        }
    }
}
