using LinkShortener.Application.Abstractions;
using LinkShortener.Domain.Entities;
using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace LinkShortener.Tests.Helpers
{
    public static class MockRepository
    {
        public static Mock<IUrlRepository> CreateDefault()
        {
            var mock = new Mock<IUrlRepository>();

            mock.Setup(r => r.IsUnique(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            mock.Setup(r => r.AddAsync(It.IsAny<Link>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            mock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            return mock;
        }
    }
}
