using LinkShortener.Api.Controllers;
using LinkShortener.Application.Features.Auth.Commands;
using LinkShortener.Application.Features.Auth.DTOs;
using LiteBus.Commands.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace LinkShortener.Tests.UnitTests.Controllers
{
    public class AuthControllerTests
    {
        [Fact]
        public async Task Register_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var mockCommandMediator = new Mock<ICommandMediator>();
            var expectedResponse = new RegisterUserResponse(
                Guid.NewGuid(),
                "testuser",
                "test@example.com",
                false);

            mockCommandMediator
                .Setup(m => m.SendAsync(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            var controller = new AuthController(mockCommandMediator.Object);
            var request = new RegisterUserRequest(
                "testuser",
                "test@example.com",
                "ValidPassword123!@#");

            // Act
            var result = await controller.Register(request, CancellationToken.None);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            mockCommandMediator.Verify(m => m.SendAsync(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Login_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var mockCommandMediator = new Mock<ICommandMediator>();
            var expectedResponse = new LoginUserResponse(
                Guid.NewGuid(),
                "testuser",
                "test@example.com",
                "mock_token",
                "mock_refresh_token");

            mockCommandMediator
                .Setup(m => m.SendAsync(It.IsAny<LoginUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            var controller = new AuthController(mockCommandMediator.Object);
            var request = new LoginUserRequest(
                "test@example.com",
                "password123");

            // Act
            var result = await controller.Login(request, CancellationToken.None);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            mockCommandMediator.Verify(m => m.SendAsync(It.IsAny<LoginUserCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RefreshToken_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var mockCommandMediator = new Mock<ICommandMediator>();
            var expectedResponse = new LoginUserResponse(
                Guid.NewGuid(),
                "testuser",
                "test@example.com",
                "new_token",
                "new_refresh_token");

            mockCommandMediator
                .Setup(m => m.SendAsync(It.IsAny<RefreshTokenCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            var controller = new AuthController(mockCommandMediator.Object);
            var request = new RefreshTokenRequest("valid_refresh_token");

            // Act
            var result = await controller.RefreshToken(request, CancellationToken.None);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            mockCommandMediator.Verify(m => m.SendAsync(It.IsAny<RefreshTokenCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task VerifyEmailCode_ValidCode_ReturnsOkResult()
        {
            // Arrange
            var mockCommandMediator = new Mock<ICommandMediator>();
            mockCommandMediator
                .Setup(m => m.SendAsync(It.IsAny<VerifyEmailCodeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var controller = new AuthController(mockCommandMediator.Object);
            var request = new VerifyEmailCodeRequest { Email = "test@example.com", Code = "123456" };

            // Act
            var result = await controller.VerifyEmailCode(request, CancellationToken.None);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task VerifyEmailCode_InvalidCode_ReturnsBadRequest()
        {
            // Arrange
            var mockCommandMediator = new Mock<ICommandMediator>();
            mockCommandMediator
                .Setup(m => m.SendAsync(It.IsAny<VerifyEmailCodeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var controller = new AuthController(mockCommandMediator.Object);
            var request = new VerifyEmailCodeRequest { Email = "test@example.com", Code = "wrong" };

            // Act
            var result = await controller.VerifyEmailCode(request, CancellationToken.None);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
