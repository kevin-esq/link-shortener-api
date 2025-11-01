using LinkShortener.Application.Abstractions;
using LinkShortener.Application.Abstractions.Security;
using LinkShortener.Application.Abstractions.Services;
using LinkShortener.Application.Features.Auth.Commands;
using LinkShortener.Application.Features.Auth.DTOs;
using LinkShortener.Domain.Entities;
using LiteBus.Commands.Abstractions;
using Microsoft.Extensions.Logging;

namespace LinkShortener.Application.Features.Auth.Handlers
{
    public class GoogleAuthCommandHandler : ICommandHandler<GoogleAuthCommand, LoginUserResponse>
    {
        private readonly IGoogleAuthService _googleAuthService;
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly IJwtService _jwtService;
        private readonly ILogger<GoogleAuthCommandHandler> _logger;

        public GoogleAuthCommandHandler(
            IGoogleAuthService googleAuthService,
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            ISessionRepository sessionRepository,
            IJwtService jwtService,
            ILogger<GoogleAuthCommandHandler> logger)
        {
            _googleAuthService = googleAuthService;
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _sessionRepository = sessionRepository;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<LoginUserResponse> HandleAsync(GoogleAuthCommand request, CancellationToken ct)
        {
            var googleUser = await _googleAuthService.ValidateGoogleTokenAsync(request.IdToken, ct);
            if (googleUser == null)
                throw new UnauthorizedAccessException("Invalid Google token");

            var user = await _userRepository.GetByEmailAsync(googleUser.Email, ct);

            if (user == null)
            {
                var username = GenerateUsername(googleUser.Name, googleUser.Email);
                user = User.CreateFromOAuth(
                    googleUser.Email,
                    username,
                    AuthProvider.Google,
                    googleUser.GoogleId
                );

                await _userRepository.AddAsync(user, ct);
                await _userRepository.SaveChangesAsync(ct);

                _logger.LogInformation("New user created via Google OAuth: {UserId}, Email: {Email}",
                    user.Id, user.Email);
            }
            else if (user.AuthProvider != AuthProvider.Google)
            {
                throw new InvalidOperationException(
                    $"An account with this email already exists using {user.AuthProvider} authentication. " +
                    "Please log in using your original method.");
            }

            if (!user.CanLogin)
            {
                if (user.Status == UserStatus.Suspended)
                    throw new UnauthorizedAccessException($"Account suspended: {user.SuspensionReason}");

                if (user.Status == UserStatus.Banned)
                    throw new UnauthorizedAccessException("Account has been permanently banned");

                throw new UnauthorizedAccessException("Account is not active");
            }

            user.UpdateLastLogin();

            var token = _jwtService.GenerateToken(user.Id, user.Email, user.Roles);
            var refreshToken = _jwtService.GenerateRefreshToken();

            var refreshTokenEntity = RefreshToken.Create(
                user.Id,
                refreshToken,
                _jwtService.GetRefreshTokenExpiration()
            );

            await _refreshTokenRepository.AddAsync(refreshTokenEntity, ct);
            await _refreshTokenRepository.SaveChangesAsync(ct);

            return new LoginUserResponse(user.Id, user.Username, user.Email, token, refreshToken);
        }

        private static string GenerateUsername(string name, string email)
        {
            var baseName = name.Replace(" ", "").ToLowerInvariant();
            if (baseName.Length > 20)
                baseName = baseName[..20];

            var emailPrefix = email.Split('@')[0];
            var random = new Random().Next(100, 999);

            return $"{baseName}{random}";
        }
    }
}
