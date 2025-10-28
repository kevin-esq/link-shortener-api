using LinkShortener.Application.Features.Auth.Commands;
using LinkShortener.Application.Features.Auth.Handlers;
using LinkShortener.Domain.Entities;
using Moq;
using Xunit;

namespace LinkShortener.Tests.UnitTests.Handlers
{
    public class LoginUserCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ValidCredentials_ReturnsLoginResponse()
        {
            // Arrange
            var mockUserRepo = Helpers.MockRepository.CreateUserRepository();
            var mockRefreshTokenRepo = Helpers.MockRepository.CreateRefreshTokenRepository();
            var mockSessionRepo = Helpers.MockRepository.CreateSessionRepository();
            var mockJwtService = Helpers.MockRepository.CreateJwtService();
            var mockPasswordHasher = Helpers.MockRepository.CreatePasswordHasher();

            var userId = Guid.NewGuid();
            var user = User.Create("testuser", "test@example.com", "hashedPassword");
            typeof(User).GetProperty("Id")?.SetValue(user, userId);
            typeof(User).GetProperty("IsEmailVerified")?.SetValue(user, true);

            mockUserRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var handler = new LoginUserCommandHandler(
                mockUserRepo.Object,
                mockRefreshTokenRepo.Object,
                mockSessionRepo.Object,
                mockJwtService.Object,
                mockPasswordHasher.Object);

            var command = new LoginUserCommand("test@example.com", "password123");

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal("mock_jwt_token", result.Token);
            Assert.Equal("mock_refresh_token", result.RefreshToken);
        }

        [Fact]
        public async Task Handle_InvalidEmail_ThrowsArgumentException()
        {
            // Arrange
            var mockUserRepo = Helpers.MockRepository.CreateUserRepository();
            var mockRefreshTokenRepo = Helpers.MockRepository.CreateRefreshTokenRepository();
            var mockSessionRepo = Helpers.MockRepository.CreateSessionRepository();
            var mockJwtService = Helpers.MockRepository.CreateJwtService();
            var mockPasswordHasher = Helpers.MockRepository.CreatePasswordHasher();

            mockUserRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            var handler = new LoginUserCommandHandler(
                mockUserRepo.Object,
                mockRefreshTokenRepo.Object,
                mockSessionRepo.Object,
                mockJwtService.Object,
                mockPasswordHasher.Object);

            var command = new LoginUserCommand("nonexistent@example.com", "password123");

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_InvalidPassword_ThrowsArgumentException()
        {
            // Arrange
            var mockUserRepo = Helpers.MockRepository.CreateUserRepository();
            var mockRefreshTokenRepo = Helpers.MockRepository.CreateRefreshTokenRepository();
            var mockSessionRepo = Helpers.MockRepository.CreateSessionRepository();
            var mockJwtService = Helpers.MockRepository.CreateJwtService();
            var mockPasswordHasher = Helpers.MockRepository.CreatePasswordHasher();

            var user = User.Create("testuser", "test@example.com", "hashedPassword");

            mockUserRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            mockPasswordHasher.Setup(h => h.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(false);

            var handler = new LoginUserCommandHandler(
                mockUserRepo.Object,
                mockRefreshTokenRepo.Object,
                mockSessionRepo.Object,
                mockJwtService.Object,
                mockPasswordHasher.Object);

            var command = new LoginUserCommand("test@example.com", "wrongpassword");

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_UnverifiedEmail_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var mockUserRepo = Helpers.MockRepository.CreateUserRepository();
            var mockRefreshTokenRepo = Helpers.MockRepository.CreateRefreshTokenRepository();
            var mockSessionRepo = Helpers.MockRepository.CreateSessionRepository();
            var mockJwtService = Helpers.MockRepository.CreateJwtService();
            var mockPasswordHasher = Helpers.MockRepository.CreatePasswordHasher();

            var user = User.Create("testuser", "test@example.com", "hashedPassword");

            mockUserRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var handler = new LoginUserCommandHandler(
                mockUserRepo.Object,
                mockRefreshTokenRepo.Object,
                mockSessionRepo.Object,
                mockJwtService.Object,
                mockPasswordHasher.Object);

            var command = new LoginUserCommand("test@example.com", "password123");

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                handler.Handle(command, CancellationToken.None));
        }
    }
}
