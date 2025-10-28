using LinkShortener.Domain.Entities;
using Xunit;

namespace LinkShortener.Tests.UnitTests.Entities
{
    public class LinkTests
    {
        [Fact]
        public void Create_ValidInput_CreatesLink()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var link = Link.Create("https://example.com", "ABC1234", userId);

            // Assert
            Assert.NotNull(link);
            Assert.Equal("https://example.com", link.LongUrl);
            Assert.Equal("ABC1234", link.Code);
            Assert.Equal(userId, link.UserId);
        }

        [Fact]
        public void Create_InvalidUrl_ThrowsArgumentException()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                Link.Create("not-a-valid-url", "ABC1234", userId));
        }

        [Fact]
        public void Create_EmptyCode_ThrowsArgumentException()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                Link.Create("https://example.com", "", userId));
        }

        [Fact]
        public void Create_CodeTooShort_ThrowsArgumentException()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                Link.Create("https://example.com", "ABC", userId));
        }

        [Fact]
        public void Create_CodeTooLong_ThrowsArgumentException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var longCode = new string('A', 21);

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                Link.Create("https://example.com", longCode, userId));
        }

        [Fact]
        public void OverrideShortUrlBase_ValidPath_SetsShortUrl()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var link = Link.Create("https://example.com", "ABC1234", userId);

            // Act
            link.OverrideShortUrlBase("https://short.link/s/");

            // Assert
            Assert.Equal("https://short.link/s/ABC1234", link.ShortUrl);
            Assert.NotNull(link.UpdatedOnUtc);
        }

        [Fact]
        public void OverrideShortUrlBase_EmptyPath_ThrowsArgumentException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var link = Link.Create("https://example.com", "ABC1234", userId);

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                link.OverrideShortUrlBase(""));
        }

        [Fact]
        public void AddAccess_ValidInput_AddsAccess()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var link = Link.Create("https://example.com", "ABC1234", userId);
            var accessUserId = Guid.NewGuid();

            // Act
            link.AddAccess(accessUserId, "192.168.1.1", "Mozilla/5.0");

            // Assert
            Assert.Single(link.Accesses);
        }

        [Fact]
        public void AddAccess_NullUserId_AddsAnonymousAccess()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var link = Link.Create("https://example.com", "ABC1234", userId);

            // Act
            link.AddAccess(null, "192.168.1.1", "Mozilla/5.0");

            // Assert
            Assert.Single(link.Accesses);
        }
    }
}
