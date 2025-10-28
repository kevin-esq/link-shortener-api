using LinkShortener.Api.Controllers;
using LinkShortener.Application.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace LinkShortener.Tests.UnitTests.Controllers
{
    public class AuditControllerTests
    {
        private readonly Mock<IAuditService> _auditServiceMock;
        private readonly AuditController _controller;

        public AuditControllerTests()
        {
            _auditServiceMock = new Mock<IAuditService>();
            _controller = new AuditController(_auditServiceMock.Object);
            SetupAdminContext();
        }

        private void SetupAdminContext()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Admin")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        [Fact]
        public async Task GetLogs_ReturnsOk_WithLogs()
        {
            var logs = new List<AuditLogDto>
            {
                new AuditLogDto(
                    Guid.NewGuid(),
                    DateTime.UtcNow,
                    Guid.NewGuid(),
                    "admin@test.com",
                    "Admin",
                    "192.168.1.1",
                    "user.create",
                    "user",
                    Guid.NewGuid(),
                    "test@example.com",
                    null,
                    "{\"role\":\"user\"}",
                    null,
                    "success",
                    null)
            };

            _auditServiceMock
                .Setup(s => s.QueryAsync(
                    It.IsAny<Guid?>(),
                    It.IsAny<string?>(),
                    It.IsAny<string?>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(logs);

            var result = await _controller.GetLogs(null, null, null, null, null, 1, 50, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;
            Assert.NotNull(response);
        }

        [Fact]
        public async Task GetLogs_WithFilters_CallsServiceWithCorrectParams()
        {
            var actorId = Guid.NewGuid();
            var fromDate = DateTime.UtcNow.AddDays(-7);
            var toDate = DateTime.UtcNow;

            _auditServiceMock
                .Setup(s => s.QueryAsync(actorId, "user.create", "user", fromDate, toDate, 1, 50, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<AuditLogDto>());

            await _controller.GetLogs(actorId, "user.create", "user", fromDate, toDate, 1, 50, CancellationToken.None);

            _auditServiceMock.Verify(s => s.QueryAsync(
                actorId,
                "user.create",
                "user",
                fromDate,
                toDate,
                1,
                50,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetLogs_ReturnsEmptyList_WhenNoLogs()
        {
            _auditServiceMock
                .Setup(s => s.QueryAsync(
                    It.IsAny<Guid?>(),
                    It.IsAny<string?>(),
                    It.IsAny<string?>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<AuditLogDto>());

            var result = await _controller.GetLogs(null, null, null, null, null, 1, 50, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public void GetAvailableActions_ReturnsOk_WithActions()
        {
            var result = _controller.GetAvailableActions();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }
    }
}
