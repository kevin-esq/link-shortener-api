using LinkShortener.Application.Abstractions;
using LinkShortener.Application.Features.Url.Handlers;
using LinkShortener.Application.Features.Url.Queries;
using LinkShortener.Domain.Entities;
using Moq;
using Xunit;

namespace LinkShortener.Tests.UnitTests.Features
{
    public class GetQrCodeQueryTests
    {
        private readonly Mock<IUrlRepository> _repositoryMock;
        private readonly Mock<IQrCodeService> _qrCodeServiceMock;
        private readonly GetQrCodeQueryHandler _handler;

        public GetQrCodeQueryTests()
        {
            _repositoryMock = new Mock<IUrlRepository>();
            _qrCodeServiceMock = new Mock<IQrCodeService>();
            _handler = new GetQrCodeQueryHandler(_repositoryMock.Object, _qrCodeServiceMock.Object);
        }

        [Fact]
        public async Task Handle_LinkNotFound_ReturnsNull()
        {
            var userId = Guid.NewGuid();

            _repositoryMock
                .Setup(r => r.GetByCodeAsync("INVALID", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Link?)null);

            var query = new GetQrCodeQuery("INVALID", userId);
            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Null(result);
        }

        [Fact]
        public async Task Handle_DifferentUser_ReturnsNull()
        {
            var userId = Guid.NewGuid();
            var differentUserId = Guid.NewGuid();
            var link = Link.Create("https://example.com", "ABC123", differentUserId);

            _repositoryMock
                .Setup(r => r.GetByCodeAsync("ABC123", It.IsAny<CancellationToken>()))
                .ReturnsAsync(link);

            var query = new GetQrCodeQuery("ABC123", userId);
            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Null(result);
        }
    }
}
