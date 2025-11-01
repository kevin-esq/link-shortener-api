using LinkShortener.Api.Controllers;
using LinkShortener.Application.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace LinkShortener.Tests.UnitTests.Controllers
{
    public class MetricsControllerTests
    {
        private readonly Mock<IClickEventService> _clickEventServiceMock;
        private readonly MetricsController _controller;

        public MetricsControllerTests()
        {
            _clickEventServiceMock = new Mock<IClickEventService>();
            _controller = new MetricsController(_clickEventServiceMock.Object);
            SetupUserContext();
        }

        private void SetupUserContext()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        [Fact]
        public async Task GetLinkStats_ReturnsOk_WithStats()
        {
            var linkId = Guid.NewGuid();
            var stats = new ClickEventStatsDto(
                100,
                75,
                5,
                45.5,
                new Dictionary<string, int> { { "MX", 50 }, { "US", 30 } },
                new Dictionary<string, int> { { "mobile", 60 }, { "desktop", 40 } },
                new Dictionary<string, int> { { "14", 30 }, { "15", 25 } });

            _clickEventServiceMock
                .Setup(s => s.GetStatsAsync(linkId, null, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(stats);

            var result = await _controller.GetLinkStats(linkId, null, null, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;
            Assert.NotNull(response);
        }

        [Fact]
        public async Task GetLinkStats_WithDateRange_CallsServiceWithDates()
        {
            var linkId = Guid.NewGuid();
            var fromDate = DateTime.UtcNow.AddDays(-7);
            var toDate = DateTime.UtcNow;

            _clickEventServiceMock
                .Setup(s => s.GetStatsAsync(linkId, fromDate, toDate, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ClickEventStatsDto(0, 0, 0, 0, new(), new(), new()));

            await _controller.GetLinkStats(linkId, fromDate, toDate, CancellationToken.None);

            _clickEventServiceMock.Verify(s => s.GetStatsAsync(linkId, fromDate, toDate, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetRecentClicks_ReturnsOk_WithClicks()
        {
            var linkId = Guid.NewGuid();
            var clicks = new List<ClickEventDto>
            {
                new ClickEventDto(
                    Guid.NewGuid(),
                    DateTime.UtcNow,
                    linkId,
                    "ABC123",
                    null,
                    "https://twitter.com",
                    "MX",
                    "CDMX",
                    "mobile",
                    "Chrome",
                    "redirected",
                    45)
            };

            _clickEventServiceMock
                .Setup(s => s.GetRecentClicksAsync(linkId, 100, It.IsAny<CancellationToken>()))
                .ReturnsAsync(clicks);

            var result = await _controller.GetRecentClicks(linkId, CancellationToken.None, 100);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetRecentClicks_CustomLimit_UsesProvidedLimit()
        {
            var linkId = Guid.NewGuid();

            _clickEventServiceMock
                .Setup(s => s.GetRecentClicksAsync(linkId, 50, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ClickEventDto>());

            await _controller.GetRecentClicks(linkId, CancellationToken.None, 50);

            _clickEventServiceMock.Verify(s => s.GetRecentClicksAsync(linkId, 50, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
