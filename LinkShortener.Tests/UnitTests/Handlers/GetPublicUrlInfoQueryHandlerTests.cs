using LinkShortener.Application.Features.Url.Handlers;
using LinkShortener.Application.Features.Url.Queries;
using LinkShortener.Domain.Entities;
using Moq;
using Xunit;

namespace LinkShortener.Tests.UnitTests.Handlers
{
    public class GetPublicUrlInfoQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ValidCode_ReturnsUrlInfo()
        {
            // Arrange
            var mockRepo = Helpers.MockRepository.CreateDefault();
            var linkId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var link = Link.Create("https://example.com", "ABC1234", userId);
            typeof(Link).GetProperty("Id")?.SetValue(link, linkId);

            mockRepo.Setup(r => r.GetByCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(link);

            var handler = new GetPublicUrlInfoQueryHandler(mockRepo.Object);
            var query = new GetPublicUrlInfoQuery("ABC1234", "https", "localhost:7205");

            // Act
            var result = await handler.HandleAsync(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(linkId, result.Id);
            Assert.Equal("https://example.com", result.OriginalUrl);
            Assert.NotNull(result.ShortUrl);
        }

        [Fact]
        public async Task Handle_InvalidCode_ReturnsNull()
        {
            // Arrange
            var mockRepo = Helpers.MockRepository.CreateDefault();

            mockRepo.Setup(r => r.GetByCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Link?)null);

            var handler = new GetPublicUrlInfoQueryHandler(mockRepo.Object);
            var query = new GetPublicUrlInfoQuery("INVALID", "https", "localhost:7205");

            // Act
            var result = await handler.HandleAsync(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Handle_EmptyCode_ThrowsArgumentException()
        {
            // Arrange
            var mockRepo = Helpers.MockRepository.CreateDefault();
            var handler = new GetPublicUrlInfoQueryHandler(mockRepo.Object);
            var query = new GetPublicUrlInfoQuery("", "https", "localhost:7205");

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                handler.HandleAsync(query, CancellationToken.None));
        }
    }
}
