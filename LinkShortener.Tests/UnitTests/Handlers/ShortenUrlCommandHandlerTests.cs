using LinkShortener.Application.Features.Url.Commands;
using LinkShortener.Application.Features.Url.Handlers;
using Xunit;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LinkShortener.Tests.UnitTests.Handlers
{
    public class ShortenUrlCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ValidUrl_ReturnsShortUrl()
        {
            // Arrange
            var mockRepo = Helpers.MockRepository.CreateDefault();
            var handler = new ShortenUrlCommandHandler(mockRepo.Object);

            var userId = Guid.NewGuid();
            var command = new ShortenUrlCommand(
                "https://example.com?param1=value1&param2=value2?param3=value3",
                "https",
                "localhost:7205",
                userId
            );

            // Act
            var result = await handler.HandleAsync(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(command.Url, result.OriginalUrl);
            Assert.NotNull(result.ShortUrl);
            Assert.Equal(7, result.Code.Length);
        }

        [Fact]
        public async Task Handle_InvalidUrl_ThrowsArgumentException()
        {
            // Arrange
            var mockRepo = Helpers.MockRepository.CreateDefault();
            var handler = new ShortenUrlCommandHandler(mockRepo.Object);

            var userId = Guid.NewGuid();
            var command = new ShortenUrlCommand(
                "not-a-url",
                "https",
                "localhost:7205",
                userId
            );

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                handler.HandleAsync(command, CancellationToken.None));
        }
    }
}
