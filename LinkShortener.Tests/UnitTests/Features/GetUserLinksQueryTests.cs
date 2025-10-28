using LinkShortener.Application.Abstractions;
using LinkShortener.Application.Features.Url.Handlers;
using LinkShortener.Application.Features.Url.Queries;
using Moq;
using Xunit;

namespace LinkShortener.Tests.UnitTests.Features
{
    public class GetUserLinksQueryTests
    {
        private readonly Mock<IUrlRepository> _repositoryMock;
        private readonly GetUserLinksQueryHandler _handler;

        public GetUserLinksQueryTests()
        {
            _repositoryMock = new Mock<IUrlRepository>();
            _handler = new GetUserLinksQueryHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsLinks_WithPagination()
        {
            var userId = Guid.NewGuid();
            var links = new List<LinkWithStats>
            {
                new LinkWithStats(
                    Guid.NewGuid(),
                    "ABC123",
                    "https://short.link/ABC123",
                    "https://example.com",
                    DateTime.UtcNow,
                    50,
                    DateTime.UtcNow)
            };

            _repositoryMock
                .Setup(r => r.GetUserLinksPagedAsync(userId, 1, 20, null, "createdAt", "desc", It.IsAny<CancellationToken>()))
                .ReturnsAsync((links, 1));

            var query = new GetUserLinksQuery(userId, 1, 20);
            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Single(result.Links);
            Assert.Equal(1, result.TotalCount);
            Assert.Equal(1, result.TotalPages);
        }

        [Fact]
        public async Task Handle_WithSearch_CallsRepositoryWithSearch()
        {
            var userId = Guid.NewGuid();
            
            _repositoryMock
                .Setup(r => r.GetUserLinksPagedAsync(userId, 1, 20, "google", "createdAt", "desc", It.IsAny<CancellationToken>()))
                .ReturnsAsync((new List<LinkWithStats>(), 0));

            var query = new GetUserLinksQuery(userId, 1, 20, "google");
            await _handler.Handle(query, CancellationToken.None);

            _repositoryMock.Verify(r => r.GetUserLinksPagedAsync(
                userId, 1, 20, "google", "createdAt", "desc", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithOrderBy_CallsRepositoryWithOrderBy()
        {
            var userId = Guid.NewGuid();
            
            _repositoryMock
                .Setup(r => r.GetUserLinksPagedAsync(userId, 1, 20, null, "clicks", "desc", It.IsAny<CancellationToken>()))
                .ReturnsAsync((new List<LinkWithStats>(), 0));

            var query = new GetUserLinksQuery(userId, 1, 20, null, "clicks", "desc");
            await _handler.Handle(query, CancellationToken.None);

            _repositoryMock.Verify(r => r.GetUserLinksPagedAsync(
                userId, 1, 20, null, "clicks", "desc", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_CalculatesTotalPages_Correctly()
        {
            var userId = Guid.NewGuid();
            
            _repositoryMock
                .Setup(r => r.GetUserLinksPagedAsync(userId, 1, 20, null, "createdAt", "desc", It.IsAny<CancellationToken>()))
                .ReturnsAsync((new List<LinkWithStats>(), 45));

            var query = new GetUserLinksQuery(userId, 1, 20);
            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Equal(45, result.TotalCount);
            Assert.Equal(3, result.TotalPages);
        }
    }
}
