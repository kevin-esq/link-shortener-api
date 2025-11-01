using LinkShortener.Application.Features.Auth.Commands;
using LinkShortener.Application.Features.Auth.Handlers;
using LinkShortener.Domain.Entities;
using Moq;
using Xunit;

namespace LinkShortener.Tests.UnitTests.Handlers
{
    public class ResetPasswordCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ValidCodeAndPassword_ResetsPassword()
        {
            // Arrange
            var mockUserRepo = Helpers.MockRepository.CreateUserRepository();
            var mockCodeStore = Helpers.MockRepository.CreateVerificationCodeStore();
            var mockPasswordHasher = Helpers.MockRepository.CreatePasswordHasher();

            var user = User.Create("testuser", "test@example.com", "oldHashedPassword");

            mockCodeStore.Setup(s => s.GetCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("123456");

            mockUserRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var handler = new ResetPasswordCommandHandler(
                mockUserRepo.Object,
                mockCodeStore.Object,
                mockPasswordHasher.Object);

            var command = new ResetPasswordCommand("test@example.com", "123456", "NewPassword123!@#");

            // Act
            await handler.HandleAsync(command, CancellationToken.None);

            // Assert
            mockUserRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            mockCodeStore.Verify(s => s.DeleteCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidCode_ThrowsArgumentException()
        {
            // Arrange
            var mockUserRepo = Helpers.MockRepository.CreateUserRepository();
            var mockCodeStore = Helpers.MockRepository.CreateVerificationCodeStore();
            var mockPasswordHasher = Helpers.MockRepository.CreatePasswordHasher();

            mockCodeStore.Setup(s => s.GetCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("123456");

            var handler = new ResetPasswordCommandHandler(
                mockUserRepo.Object,
                mockCodeStore.Object,
                mockPasswordHasher.Object);

            var command = new ResetPasswordCommand("test@example.com", "wrong_code", "NewPassword123!@#");

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                handler.HandleAsync(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WeakPassword_ThrowsArgumentException()
        {
            // Arrange
            var mockUserRepo = Helpers.MockRepository.CreateUserRepository();
            var mockCodeStore = Helpers.MockRepository.CreateVerificationCodeStore();
            var mockPasswordHasher = Helpers.MockRepository.CreatePasswordHasher();

            var user = User.Create("testuser", "test@example.com", "oldHashedPassword");

            mockCodeStore.Setup(s => s.GetCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("123456");

            mockUserRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var handler = new ResetPasswordCommandHandler(
                mockUserRepo.Object,
                mockCodeStore.Object,
                mockPasswordHasher.Object);

            var command = new ResetPasswordCommand("test@example.com", "123456", "weak");

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                handler.HandleAsync(command, CancellationToken.None));
        }
    }
}
