using LinkShortener.Application.Features.Auth.Commands;
using LinkShortener.Application.Features.Auth.Handlers;
using LinkShortener.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LinkShortener.Tests.UnitTests.Handlers
{
    public class VerifyEmailCodeHandlerTests
    {
        [Fact]
        public async Task Handle_ValidCode_ReturnsTrue()
        {
            // Arrange
            var mockCodeStore = Helpers.MockRepository.CreateVerificationCodeStore();
            var mockUserRepo = Helpers.MockRepository.CreateUserRepository();
            var mockLogger = new Mock<ILogger<VerifyEmailCodeHandler>>();

            var user = User.Create("testuser", "test@example.com", "hashedPassword");

            mockCodeStore.Setup(s => s.GetCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("123456");

            mockUserRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var handler = new VerifyEmailCodeHandler(
                mockCodeStore.Object,
                mockUserRepo.Object,
                mockLogger.Object);

            var command = new VerifyEmailCodeCommand("test@example.com", "123456");

            // Act
            var result = await handler.HandleAsync(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            mockUserRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            mockCodeStore.Verify(s => s.DeleteCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidCode_ReturnsFalse()
        {
            // Arrange
            var mockCodeStore = Helpers.MockRepository.CreateVerificationCodeStore();
            var mockUserRepo = Helpers.MockRepository.CreateUserRepository();
            var mockLogger = new Mock<ILogger<VerifyEmailCodeHandler>>();

            mockCodeStore.Setup(s => s.GetCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("123456");

            var handler = new VerifyEmailCodeHandler(
                mockCodeStore.Object,
                mockUserRepo.Object,
                mockLogger.Object);

            var command = new VerifyEmailCodeCommand("test@example.com", "wrong_code");

            // Act
            var result = await handler.HandleAsync(command, CancellationToken.None);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task Handle_ExpiredCode_ReturnsFalse()
        {
            // Arrange
            var mockCodeStore = Helpers.MockRepository.CreateVerificationCodeStore();
            var mockUserRepo = Helpers.MockRepository.CreateUserRepository();
            var mockLogger = new Mock<ILogger<VerifyEmailCodeHandler>>();

            mockCodeStore.Setup(s => s.GetCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((string?)null);

            var handler = new VerifyEmailCodeHandler(
                mockCodeStore.Object,
                mockUserRepo.Object,
                mockLogger.Object);

            var command = new VerifyEmailCodeCommand("test@example.com", "123456");

            // Act
            var result = await handler.HandleAsync(command, CancellationToken.None);

            // Assert
            Assert.False(result);
        }
    }
}
