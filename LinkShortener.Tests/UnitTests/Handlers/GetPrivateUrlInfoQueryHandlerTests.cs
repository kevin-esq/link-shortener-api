using LinkShortener.Application.Features.Url.Handlers;
using LinkShortener.Application.Features.Url.Queries;
using LinkShortener.Domain.Entities;
using Moq;
using Xunit;

namespace LinkShortener.Tests.UnitTests.Handlers
{
    public class GetPrivateUrlInfoQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ValidCodeAndOwner_ReturnsUrlInfo()
        {
            // Arrange
            var mockRepo = Helpers.MockRepository.CreateDefault();
            var userId = Guid.NewGuid();
            var linkId = Guid.NewGuid();
            var link = Link.Create("https://example.com", "ABC1234", userId);
            typeof(Link).GetProperty("Id")?.SetValue(link, linkId);

            mockRepo.Setup(r => r.GetByCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(link);

            var handler = new GetPrivateUrlInfoQueryHandler(mockRepo.Object);
            var query = new GetPrivateUrlInfoQuery("ABC1234", "https", "localhost:7205", userId);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(linkId, result.Id);
            Assert.Equal("https://example.com", result.OriginalUrl);
        }

        [Fact]
        public async Task Handle_DifferentOwner_ReturnsNull()
        {
            // Arrange
            var mockRepo = Helpers.MockRepository.CreateDefault();
            var ownerId = Guid.NewGuid();
            var differentUserId = Guid.NewGuid();
            var link = Link.Create("https://example.com", "ABC1234", ownerId);

            mockRepo.Setup(r => r.GetByCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(link);

            var handler = new GetPrivateUrlInfoQueryHandler(mockRepo.Object);
            var query = new GetPrivateUrlInfoQuery("ABC1234", "https", "localhost:7205", differentUserId);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Handle_InvalidCode_ReturnsNull()
        {
            // Arrange
            var mockRepo = Helpers.MockRepository.CreateDefault();
            var userId = Guid.NewGuid();

            mockRepo.Setup(r => r.GetByCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Link?)null);

            var handler = new GetPrivateUrlInfoQueryHandler(mockRepo.Object);
            var query = new GetPrivateUrlInfoQuery("INVALID", "https", "localhost:7205", userId);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Handle_EmptyCode_ThrowsArgumentException()
        {
            // Arrange
            var mockRepo = Helpers.MockRepository.CreateDefault();
            var userId = Guid.NewGuid();
            var handler = new GetPrivateUrlInfoQueryHandler(mockRepo.Object);
            var query = new GetPrivateUrlInfoQuery("", "https", "localhost:7205", userId);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                handler.Handle(query, CancellationToken.None));
        }
    }
}
