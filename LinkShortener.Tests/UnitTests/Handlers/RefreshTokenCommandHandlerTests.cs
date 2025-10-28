using LinkShortener.Application.Features.Auth.Commands;
using LinkShortener.Application.Features.Auth.Handlers;
using LinkShortener.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LinkShortener.Tests.UnitTests.Handlers
{
    public class RefreshTokenCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ValidRefreshToken_ReturnsNewTokens()
        {
            // Arrange
            var mockRefreshTokenRepo = Helpers.MockRepository.CreateRefreshTokenRepository();
            var mockJwtService = Helpers.MockRepository.CreateJwtService();
            var mockLogger = new Mock<ILogger<RefreshTokenCommandHandler>>();

            var userId = Guid.NewGuid();
            var user = User.Create("testuser", "test@example.com", "hashedPassword");
            typeof(User).GetProperty("Id")?.SetValue(user, userId);
            typeof(User).GetProperty("IsEmailVerified")?.SetValue(user, true);

            var refreshToken = RefreshToken.Create(userId, "valid_refresh_token", DateTime.UtcNow.AddDays(7));
            typeof(RefreshToken).GetProperty("User")?.SetValue(refreshToken, user);

            mockRefreshTokenRepo.Setup(r => r.GetByTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(refreshToken);

            var handler = new RefreshTokenCommandHandler(
                mockRefreshTokenRepo.Object,
                mockJwtService.Object,
                mockLogger.Object);

            var command = new RefreshTokenCommand("valid_refresh_token");

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("mock_jwt_token", result.Token);
            Assert.Equal("mock_refresh_token", result.RefreshToken);
            mockRefreshTokenRepo.Verify(r => r.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidRefreshToken_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var mockRefreshTokenRepo = Helpers.MockRepository.CreateRefreshTokenRepository();
            var mockJwtService = Helpers.MockRepository.CreateJwtService();
            var mockLogger = new Mock<ILogger<RefreshTokenCommandHandler>>();

            mockRefreshTokenRepo.Setup(r => r.GetByTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((RefreshToken?)null);

            var handler = new RefreshTokenCommandHandler(
                mockRefreshTokenRepo.Object,
                mockJwtService.Object,
                mockLogger.Object);

            var command = new RefreshTokenCommand("invalid_token");

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                handler.Handle(command, CancellationToken.None));
        }
    }
}
