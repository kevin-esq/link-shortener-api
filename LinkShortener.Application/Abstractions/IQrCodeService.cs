namespace LinkShortener.Application.Abstractions
{
    public interface IQrCodeService
    {
        byte[] GenerateQrCode(string url, int size = 300);
        string GenerateQrCodeBase64(string url, int size = 300);
    }
}
