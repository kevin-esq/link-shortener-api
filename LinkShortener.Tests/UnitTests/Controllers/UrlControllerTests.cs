using LinkShortener.Api.Controllers;
using LinkShortener.Application.Abstractions;
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

            var mockClickEventService = new Mock<IClickEventService>();
            var controller = new UrlController(mockMediator.Object, mockClickEventService.Object);

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
            var mockClickEventService = new Mock<IClickEventService>();
            var controller = new UrlController(mockMediator.Object, mockClickEventService.Object);
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

            var mockClickEventService = new Mock<IClickEventService>();
            var controller = new UrlController(mockMediator.Object, mockClickEventService.Object);
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

            var mockClickEventService = new Mock<IClickEventService>();
            var controller = new UrlController(mockMediator.Object, mockClickEventService.Object);
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

            var mockClickEventService = new Mock<IClickEventService>();
            var controller = new UrlController(mockMediator.Object, mockClickEventService.Object);

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

            var mockClickEventService = new Mock<IClickEventService>();
            var controller = new UrlController(mockMediator.Object, mockClickEventService.Object);

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

        [Fact]
        public async Task GetMyLinks_ReturnsOk_WithPaginatedLinks()
        {
            var mockMediator = new Mock<IMediator>();
            var mockClickEventService = new Mock<IClickEventService>();
            var userId = Guid.NewGuid();

            var response = new UserLinksResponse(
                new List<LinkSummaryDto>
                {
                    new LinkSummaryDto(
                        Guid.NewGuid(),
                        "ABC123",
                        "https://short.link/ABC123",
                        "https://example.com/long-url",
                        DateTime.UtcNow,
                        42,
                        DateTime.UtcNow)
                },
                1,
                1,
                20,
                1);

            mockMediator
                .Setup(m => m.Send(It.IsAny<GetUserLinksQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var controller = new UrlController(mockMediator.Object, mockClickEventService.Object);

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

            var result = await controller.GetMyLinks(1, 20, null, "createdAt", "desc", CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResponse = Assert.IsType<UserLinksResponse>(okResult.Value);
            Assert.Single(returnedResponse.Links);
            Assert.Equal(42, returnedResponse.Links[0].TotalClicks);
        }

        [Fact]
        public async Task DeleteLink_ValidCode_ReturnsNoContent()
        {
            var mockMediator = new Mock<IMediator>();
            var mockClickEventService = new Mock<IClickEventService>();
            var userId = Guid.NewGuid();

            mockMediator
                .Setup(m => m.Send(It.IsAny<DeleteLinkCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var controller = new UrlController(mockMediator.Object, mockClickEventService.Object);

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

            var result = await controller.DeleteLink("ABC123", CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
            mockMediator.Verify(m => m.Send(
                It.Is<DeleteLinkCommand>(cmd => cmd.Code == "ABC123" && cmd.UserId == userId),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteLink_NotFound_ReturnsNotFound()
        {
            var mockMediator = new Mock<IMediator>();
            var mockClickEventService = new Mock<IClickEventService>();
            var userId = Guid.NewGuid();

            mockMediator
                .Setup(m => m.Send(It.IsAny<DeleteLinkCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var controller = new UrlController(mockMediator.Object, mockClickEventService.Object);

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

            var result = await controller.DeleteLink("INVALID", CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetLinkStats_ValidCode_ReturnsStats()
        {
            var mockMediator = new Mock<IMediator>();
            var mockClickEventService = new Mock<IClickEventService>();
            var userId = Guid.NewGuid();

            var response = new LinkStatsResponse(
                "ABC123",
                "https://example.com",
                100,
                75,
                DateTime.UtcNow,
                new List<DailyClicksDto>(),
                new List<CountryClicksDto> { new CountryClicksDto("MX", 50, 50.0) },
                new List<DeviceClicksDto> { new DeviceClicksDto("mobile", 60, 60.0) },
                new List<BrowserClicksDto> { new BrowserClicksDto("Chrome", 80, 80.0) },
                new List<RefererClicksDto>());

            mockMediator
                .Setup(m => m.Send(It.IsAny<GetLinkStatsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var controller = new UrlController(mockMediator.Object, mockClickEventService.Object);

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

            var result = await controller.GetLinkStats("ABC123", 30, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var stats = Assert.IsType<LinkStatsResponse>(okResult.Value);
            Assert.Equal(100, stats.TotalClicks);
            Assert.Equal(75, stats.UniqueVisitors);
            Assert.Single(stats.ClicksByCountry);
        }

        [Fact]
        public async Task GetLinkStats_NotFound_ReturnsNotFound()
        {
            var mockMediator = new Mock<IMediator>();
            var mockClickEventService = new Mock<IClickEventService>();
            var userId = Guid.NewGuid();

            mockMediator
                .Setup(m => m.Send(It.IsAny<GetLinkStatsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((LinkStatsResponse?)null);

            var controller = new UrlController(mockMediator.Object, mockClickEventService.Object);

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

            var result = await controller.GetLinkStats("INVALID", 30, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
