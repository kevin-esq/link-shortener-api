using LinkShortener.Application.Features.Auth.Commands;
using LinkShortener.Application.Features.Auth.Handlers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LinkShortener.Tests.UnitTests.Handlers
{
    public class RegisterUserCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ValidUser_ReturnsRegisterResponse()
        {
            // Arrange
            var mockUserRepo = Helpers.MockRepository.CreateUserRepository();
            var mockEmailService = Helpers.MockRepository.CreateEmailService();
            var mockCodeStore = Helpers.MockRepository.CreateVerificationCodeStore();
            var mockPasswordHasher = Helpers.MockRepository.CreatePasswordHasher();
            var mockLogger = new Mock<ILogger<RegisterUserCommandHandler>>();

            var handler = new RegisterUserCommandHandler(
                mockUserRepo.Object,
                mockEmailService.Object,
                mockCodeStore.Object,
                mockPasswordHasher.Object,
                mockLogger.Object);

            var command = new RegisterUserCommand(
                "testuser",
                "test@example.com",
                "ValidPassword123!@#");

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result.Username);
            Assert.Equal("test@example.com", result.Email);
            Assert.False(result.IsEmailVerified);

            mockUserRepo.Verify(r => r.AddAsync(It.IsAny<LinkShortener.Domain.Entities.User>(), It.IsAny<CancellationToken>()), Times.Once);
            mockEmailService.Verify(s => s.SendAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ExistingEmail_ThrowsArgumentException()
        {
            // Arrange
            var mockUserRepo = Helpers.MockRepository.CreateUserRepository();
            var mockEmailService = Helpers.MockRepository.CreateEmailService();
            var mockCodeStore = Helpers.MockRepository.CreateVerificationCodeStore();
            var mockPasswordHasher = Helpers.MockRepository.CreatePasswordHasher();
            var mockLogger = new Mock<ILogger<RegisterUserCommandHandler>>();

            mockUserRepo.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var handler = new RegisterUserCommandHandler(
                mockUserRepo.Object,
                mockEmailService.Object,
                mockCodeStore.Object,
                mockPasswordHasher.Object,
                mockLogger.Object);

            var command = new RegisterUserCommand(
                "testuser",
                "existing@example.com",
                "ValidPassword123!@#");

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WeakPassword_ThrowsArgumentException()
        {
            // Arrange
            var mockUserRepo = Helpers.MockRepository.CreateUserRepository();
            var mockEmailService = Helpers.MockRepository.CreateEmailService();
            var mockCodeStore = Helpers.MockRepository.CreateVerificationCodeStore();
            var mockPasswordHasher = Helpers.MockRepository.CreatePasswordHasher();
            var mockLogger = new Mock<ILogger<RegisterUserCommandHandler>>();

            var handler = new RegisterUserCommandHandler(
                mockUserRepo.Object,
                mockEmailService.Object,
                mockCodeStore.Object,
                mockPasswordHasher.Object,
                mockLogger.Object);

            var command = new RegisterUserCommand(
                "testuser",
                "test@example.com",
                "weak");

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                handler.Handle(command, CancellationToken.None));
        }
    }
}
