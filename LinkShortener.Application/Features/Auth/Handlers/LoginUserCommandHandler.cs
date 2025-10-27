using LinkShortener.Application.Abstractions;
using LinkShortener.Application.Abstractions.Security;
using LinkShortener.Application.Features.Auth.Commands;
using LinkShortener.Application.Features.Auth.DTOs;
using LinkShortener.Domain.Entities;
using System.Security.Cryptography;
using MediatR;
using System.Text;

namespace LinkShortener.Application.Features.Auth.Handlers
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginUserResponse>
    {
        private readonly IUserRepository _repository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly IJwtService _tokenService;

        public LoginUserCommandHandler(
            IUserRepository repository,
            IRefreshTokenRepository refreshTokenRepository,
            ISessionRepository sessionRepository,
            IJwtService tokenService)
        {
            _repository = repository;
            _refreshTokenRepository = refreshTokenRepository;
            _sessionRepository = sessionRepository;
            _tokenService = tokenService;
        }

        public async Task<LoginUserResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _repository.GetByEmailAsync(request.Email, cancellationToken);
            if (user == null)
                throw new ArgumentException("Invalid email or password");

            var passwordHash = HashPassword(request.Password);
            if (user.PasswordHash != passwordHash)
                throw new ArgumentException("Invalid email or password");

            if (!user.CanLogin)
            {
                if (!user.IsEmailVerified)
                    throw new UnauthorizedAccessException(
                        "Email not verified. Please verify your email before logging in. " +
                        "Check your inbox for the verification code or request a new one");

                if (user.Status == UserStatus.Suspended)
                    throw new UnauthorizedAccessException(
                        $"Account suspended: {user.SuspensionReason}");

                if (user.Status == UserStatus.Banned)
                    throw new UnauthorizedAccessException("Account has been permanently banned");

                throw new UnauthorizedAccessException("Account is not active");
            }

            user.UpdateLastLogin();

            var token = _tokenService.GenerateToken(user.Id, user.Email, user.Roles);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var refreshTokenEntity = RefreshToken.Create(
                user.Id,
                refreshToken,
                _tokenService.GetRefreshTokenExpiration()
            );

            await _refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);
            await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

            return new LoginUserResponse(user.Id, user.Username, user.Email, token, refreshToken);
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
