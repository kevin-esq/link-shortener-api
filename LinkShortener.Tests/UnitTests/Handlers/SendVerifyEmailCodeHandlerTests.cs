using LinkShortener.Application.Features.Auth.Commands;
using LinkShortener.Application.Features.Auth.Handlers;
using LinkShortener.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LinkShortener.Tests.UnitTests.Handlers
{
    public class SendVerifyEmailCodeHandlerTests
    {
        [Fact]
        public async Task Handle_UnverifiedUser_SendsCodeSuccessfully()
        {
            // Arrange
            var mockEmailService = Helpers.MockRepository.CreateEmailService();
            var mockCodeStore = Helpers.MockRepository.CreateVerificationCodeStore();
            var mockUserRepo = Helpers.MockRepository.CreateUserRepository();
            var mockLogger = new Mock<ILogger<SendVerifyEmailCodeHandler>>();

            var user = User.Create("testuser", "test@example.com", "hashedPassword");

            mockUserRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var handler = new SendVerifyEmailCodeHandler(
                mockEmailService.Object,
                mockCodeStore.Object,
                mockUserRepo.Object,
                mockLogger.Object);

            var command = new SendVerifyEmailCodeCommand("test@example.com");

            // Act
            await handler.HandleAsync(command, CancellationToken.None);

            // Assert
            mockCodeStore.Verify(s => s.SaveCodeAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()), Times.Once);

            mockEmailService.Verify(s => s.SendAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public async Task Handle_NonExistentEmail_ThrowsArgumentException()
        {
            // Arrange
            var mockEmailService = Helpers.MockRepository.CreateEmailService();
            var mockCodeStore = Helpers.MockRepository.CreateVerificationCodeStore();
            var mockUserRepo = Helpers.MockRepository.CreateUserRepository();
            var mockLogger = new Mock<ILogger<SendVerifyEmailCodeHandler>>();

            mockUserRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            var handler = new SendVerifyEmailCodeHandler(
                mockEmailService.Object,
                mockCodeStore.Object,
                mockUserRepo.Object,
                mockLogger.Object);

            var command = new SendVerifyEmailCodeCommand("nonexistent@example.com");

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                handler.HandleAsync(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_AlreadyVerifiedEmail_ThrowsInvalidOperationException()
        {
            // Arrange
            var mockEmailService = Helpers.MockRepository.CreateEmailService();
            var mockCodeStore = Helpers.MockRepository.CreateVerificationCodeStore();
            var mockUserRepo = Helpers.MockRepository.CreateUserRepository();
            var mockLogger = new Mock<ILogger<SendVerifyEmailCodeHandler>>();

            var user = User.Create("testuser", "test@example.com", "hashedPassword");
            user.VerifyEmail();

            mockUserRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var handler = new SendVerifyEmailCodeHandler(
                mockEmailService.Object,
                mockCodeStore.Object,
                mockUserRepo.Object,
                mockLogger.Object);

            var command = new SendVerifyEmailCodeCommand("test@example.com");

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                handler.HandleAsync(command, CancellationToken.None));
        }
    }
}
