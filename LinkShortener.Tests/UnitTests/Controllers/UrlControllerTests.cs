using LinkShortener.Api.Controllers;
using LinkShortener.Application.Features.Url.Commands;
using LinkShortener.Application.Features.Url.DTOs;
using LinkShortener.Application.Features.Url.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace LinkShortener.Tests.UnitTests.Controllers
{
    public class UrlControllerTests
    {
        [Fact]
        public async Task ShortenUrl_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var userId = Guid.NewGuid();
            var expectedResponse = new ShortenUrlResponse
            {
                OriginalUrl = "https://example.com",
                Code = "ABC1234",
                ShortUrl = "https://localhost:7205/s/ABC1234"
            };

            mockMediator
                .Setup(m => m.Send(It.IsAny<ShortenUrlCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            var controller = new UrlController(mockMediator.Object);

            // Setup HttpContext with user claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var request = new ShortenUrlRequest { Url = "https://example.com" };

            // Act
            var result = await controller.ShortenUrl(request, CancellationToken.None);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            mockMediator.Verify(m => m.Send(It.IsAny<ShortenUrlCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ShortenUrl_EmptyUrl_ReturnsBadRequest()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var controller = new UrlController(mockMediator.Object);
            var request = new ShortenUrlRequest { Url = "" };

            // Act
            var result = await controller.ShortenUrl(request, CancellationToken.None);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task RedirectToLongUrl_ValidCode_ReturnsRedirect()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var response = new GetUrlInfoResponse
            {
                Id = Guid.NewGuid(),
                ShortUrl = "https://localhost/s/ABC1234",
                OriginalUrl = "https://example.com",
                CreatedAt = DateTime.UtcNow
            };

            mockMediator
                .Setup(m => m.Send(It.IsAny<GetPublicUrlInfoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            mockMediator
                .Setup(m => m.Send(It.IsAny<RegisterLinkAccessCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Unit.Value));

            var controller = new UrlController(mockMediator.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = await controller.RedirectToLongUrl("ABC1234");

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("https://example.com", redirectResult.Url);
        }

        [Fact]
        public async Task RedirectToLongUrl_InvalidCode_ReturnsNotFound()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();

            mockMediator
                .Setup(m => m.Send(It.IsAny<GetPublicUrlInfoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetUrlInfoResponse?)null);

            var controller = new UrlController(mockMediator.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = await controller.RedirectToLongUrl("INVALID");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetInfo_ValidCode_ReturnsOkResult()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var userId = Guid.NewGuid();
            var response = new GetUrlInfoResponse
            {
                Id = Guid.NewGuid(),
                ShortUrl = "https://localhost/s/ABC1234",
                OriginalUrl = "https://example.com",
                CreatedAt = DateTime.UtcNow
            };

            mockMediator
                .Setup(m => m.Send(It.IsAny<GetPrivateUrlInfoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var controller = new UrlController(mockMediator.Object);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await controller.GetInfo("ABC1234", CancellationToken.None);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetInfo_InvalidCode_ReturnsNotFound()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var userId = Guid.NewGuid();

            mockMediator
                .Setup(m => m.Send(It.IsAny<GetPrivateUrlInfoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetUrlInfoResponse?)null);

            var controller = new UrlController(mockMediator.Object);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await controller.GetInfo("INVALID", CancellationToken.None);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
