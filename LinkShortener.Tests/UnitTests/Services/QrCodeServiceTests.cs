using LinkShortener.Infrastructure.Services;
using Xunit;

namespace LinkShortener.Tests.UnitTests.Services
{
    public class QrCodeServiceTests
    {
        private readonly QrCodeService _service;

        public QrCodeServiceTests()
        {
            _service = new QrCodeService();
        }

        [Fact]
        public void GenerateQrCode_ValidUrl_ReturnsBytes()
        {
            var url = "https://example.com";

            var result = _service.GenerateQrCode(url, 300);

            Assert.NotNull(result);
            Assert.True(result.Length > 0);
        }

        [Fact]
        public void GenerateQrCode_CustomSize_ReturnsBytes()
        {
            var url = "https://example.com";

            var result = _service.GenerateQrCode(url, 500);

            Assert.NotNull(result);
            Assert.True(result.Length > 0);
        }

        [Fact]
        public void GenerateQrCodeBase64_ValidUrl_ReturnsBase64String()
        {
            var url = "https://example.com";

            var result = _service.GenerateQrCodeBase64(url, 300);

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.True(IsBase64String(result));
        }

        [Fact]
        public void GenerateQrCode_LongUrl_ReturnsBytes()
        {
            var url = "https://example.com/very/long/path/with/many/segments/and/query?param1=value1&param2=value2&param3=value3";

            var result = _service.GenerateQrCode(url);

            Assert.NotNull(result);
            Assert.True(result.Length > 0);
        }

        private bool IsBase64String(string base64)
        {
            if (string.IsNullOrEmpty(base64))
                return false;

            try
            {
                Convert.FromBase64String(base64);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
