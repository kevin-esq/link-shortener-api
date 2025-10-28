using LinkShortener.Application.Abstractions;
using LinkShortener.Application.Abstractions.Security;
using LinkShortener.Application.Abstractions.Services;
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

        public static Mock<IUserRepository> CreateUserRepository()
        {
            var mock = new Mock<IUserRepository>();

            mock.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            mock.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            mock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            return mock;
        }

        public static Mock<IRefreshTokenRepository> CreateRefreshTokenRepository()
        {
            var mock = new Mock<IRefreshTokenRepository>();

            mock.Setup(r => r.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            mock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            return mock;
        }

        public static Mock<ISessionRepository> CreateSessionRepository()
        {
            var mock = new Mock<ISessionRepository>();

            mock.Setup(r => r.AddAsync(It.IsAny<Session>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            mock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            return mock;
        }

        public static Mock<IPasswordHasher> CreatePasswordHasher()
        {
            var mock = new Mock<IPasswordHasher>();

            mock.Setup(h => h.HashPassword(It.IsAny<string>()))
                .Returns("hashed_password");

            mock.Setup(h => h.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            return mock;
        }

        public static Mock<IJwtService> CreateJwtService()
        {
            var mock = new Mock<IJwtService>();

            mock.Setup(s => s.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<IReadOnlyCollection<Role>>()))
                .Returns("mock_jwt_token");

            mock.Setup(s => s.GenerateRefreshToken())
                .Returns("mock_refresh_token");

            mock.Setup(s => s.GetRefreshTokenExpiration())
                .Returns(DateTime.UtcNow.AddDays(7));

            return mock;
        }

        public static Mock<IEmailService> CreateEmailService()
        {
            var mock = new Mock<IEmailService>();

            mock.Setup(s => s.SendAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>()))
                .Returns(Task.CompletedTask);

            return mock;
        }

        public static Mock<IVerificationCodeStore> CreateVerificationCodeStore()
        {
            var mock = new Mock<IVerificationCodeStore>();

            mock.Setup(s => s.SaveCodeAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<TimeSpan>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            mock.Setup(s => s.GetCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("123456");

            mock.Setup(s => s.DeleteCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            return mock;
        }
    }
}
