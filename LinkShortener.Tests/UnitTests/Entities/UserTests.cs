using LinkShortener.Domain.Entities;
using Xunit;

namespace LinkShortener.Tests.UnitTests.Entities
{
    public class UserTests
    {
        [Fact]
        public void Create_ValidInput_CreatesUser()
        {
            // Act
            var user = User.Create("testuser", "test@example.com", "hashedPassword");

            // Assert
            Assert.NotNull(user);
            Assert.Equal("testuser", user.Username);
            Assert.Equal("test@example.com", user.Email);
            Assert.True(user.IsActive);
            Assert.False(user.IsEmailVerified);
            Assert.Equal(UserStatus.Active, user.Status);
            Assert.Equal(AuthProvider.Local, user.AuthProvider);
        }

        [Fact]
        public void Create_InvalidUsername_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                User.Create("ab", "test@example.com", "hashedPassword"));
        }

        [Fact]
        public void Create_InvalidEmail_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                User.Create("testuser", "notanemail", "hashedPassword"));
        }

        [Fact]
        public void CreateFromOAuth_ValidInput_CreatesVerifiedUser()
        {
            // Act
            var user = User.CreateFromOAuth(
                "test@gmail.com",
                "testuser",
                AuthProvider.Google,
                "google_12345");

            // Assert
            Assert.NotNull(user);
            Assert.True(user.IsEmailVerified);
            Assert.NotNull(user.EmailVerifiedAt);
            Assert.Equal(AuthProvider.Google, user.AuthProvider);
            Assert.Equal("google_12345", user.ExternalProviderId);
        }

        [Fact]
        public void VerifyEmail_SetsEmailVerifiedTrue()
        {
            // Arrange
            var user = User.Create("testuser", "test@example.com", "hashedPassword");

            // Act
            user.VerifyEmail();

            // Assert
            Assert.True(user.IsEmailVerified);
            Assert.NotNull(user.EmailVerifiedAt);
        }

        [Fact]
        public void UnverifyEmail_SetsEmailVerifiedFalse()
        {
            // Arrange
            var user = User.Create("testuser", "test@example.com", "hashedPassword");
            user.VerifyEmail();

            // Act
            user.UnverifyEmail();

            // Assert
            Assert.False(user.IsEmailVerified);
            Assert.Null(user.EmailVerifiedAt);
        }

        [Fact]
        public void Suspend_UpdatesStatusAndReason()
        {
            // Arrange
            var user = User.Create("testuser", "test@example.com", "hashedPassword");

            // Act
            user.Suspend("Violation of terms");

            // Assert
            Assert.Equal(UserStatus.Suspended, user.Status);
            Assert.Equal("Violation of terms", user.SuspensionReason);
            Assert.NotNull(user.SuspendedAt);
        }

        [Fact]
        public void Ban_DeactivatesAndBansUser()
        {
            // Arrange
            var user = User.Create("testuser", "test@example.com", "hashedPassword");

            // Act
            user.Ban("Spam behavior");

            // Assert
            Assert.Equal(UserStatus.Banned, user.Status);
            Assert.False(user.IsActive);
            Assert.Equal("Spam behavior", user.SuspensionReason);
        }

        [Fact]
        public void Unsuspend_RestoresActiveStatus()
        {
            // Arrange
            var user = User.Create("testuser", "test@example.com", "hashedPassword");
            user.Suspend("Test suspension");

            // Act
            user.Unsuspend();

            // Assert
            Assert.Equal(UserStatus.Active, user.Status);
            Assert.Null(user.SuspensionReason);
            Assert.Null(user.SuspendedAt);
        }

        [Fact]
        public void AddRole_AddsRoleToUser()
        {
            // Arrange
            var user = User.Create("testuser", "test@example.com", "hashedPassword");

            // Act
            user.AddRole(Role.Admin);

            // Assert
            Assert.True(user.HasRole(Role.Admin));
            Assert.True(user.IsAdmin);
        }

        [Fact]
        public void RemoveRole_RemovesRoleFromUser()
        {
            // Arrange
            var user = User.Create("testuser", "test@example.com", "hashedPassword");
            user.AddRole(Role.Admin);

            // Act
            user.RemoveRole(Role.Admin);

            // Assert
            Assert.False(user.HasRole(Role.Admin));
            Assert.False(user.IsAdmin);
        }

        [Fact]
        public void CanLogin_UnverifiedEmail_ReturnsFalse()
        {
            // Arrange
            var user = User.Create("testuser", "test@example.com", "hashedPassword");

            // Assert
            Assert.False(user.CanLogin);
        }

        [Fact]
        public void CanLogin_VerifiedEmail_ReturnsTrue()
        {
            // Arrange
            var user = User.Create("testuser", "test@example.com", "hashedPassword");
            user.VerifyEmail();

            // Assert
            Assert.True(user.CanLogin);
        }

        [Fact]
        public void CanLogin_SuspendedUser_ReturnsFalse()
        {
            // Arrange
            var user = User.Create("testuser", "test@example.com", "hashedPassword");
            user.VerifyEmail();
            user.Suspend("Test");

            // Assert
            Assert.False(user.CanLogin);
        }

        [Fact]
        public void UpdateEmail_ChangesEmailAndUnverifies()
        {
            // Arrange
            var user = User.Create("testuser", "old@example.com", "hashedPassword");
            user.VerifyEmail();

            // Act
            user.UpdateEmail("new@example.com");

            // Assert
            Assert.Equal("new@example.com", user.Email);
            Assert.False(user.IsEmailVerified);
        }

        [Fact]
        public void ChangePassword_UpdatesPasswordHash()
        {
            // Arrange
            var user = User.Create("testuser", "test@example.com", "oldHash");

            // Act
            user.ChangePassword("newHash");

            // Assert
            Assert.Equal("newHash", user.PasswordHash);
        }

        [Fact]
        public void UpdateLastLogin_SetsLastLoginTime()
        {
            // Arrange
            var user = User.Create("testuser", "test@example.com", "hashedPassword");

            // Act
            user.UpdateLastLogin();

            // Assert
            Assert.NotNull(user.LastLoginAt);
        }
    }
}
