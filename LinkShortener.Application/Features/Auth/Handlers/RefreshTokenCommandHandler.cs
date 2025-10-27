using LinkShortener.Application.Abstractions;
using LinkShortener.Application.Abstractions.Security;
using LinkShortener.Application.Features.Auth.Commands;
using LinkShortener.Application.Features.Auth.DTOs;
using LinkShortener.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LinkShortener.Application.Features.Auth.Handlers
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, LoginUserResponse>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtService _jwtService;
        private readonly ILogger<RefreshTokenCommandHandler> _logger;

        public RefreshTokenCommandHandler(
            IRefreshTokenRepository refreshTokenRepository,
            IJwtService jwtService,
            ILogger<RefreshTokenCommandHandler> logger)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<LoginUserResponse> Handle(RefreshTokenCommand request, CancellationToken ct)
        {
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, ct);

            if (storedToken == null)
            {
                _logger.LogWarning("Refresh token not found: {Token}", request.RefreshToken);
                throw new UnauthorizedAccessException("Invalid refresh token");
            }

            if (!storedToken.IsValid)
            {
                _logger.LogWarning("Invalid refresh token for user: {UserId}, Revoked: {IsRevoked}, Used: {IsUsed}, Expired: {IsExpired}",
                    storedToken.UserId, storedToken.IsRevoked, storedToken.IsUsed, storedToken.ExpiresAt < DateTime.UtcNow);
                throw new UnauthorizedAccessException("Refresh token is revoked, used, or expired");
            }

            var user = storedToken.User;
            if (!user.IsActive)
                throw new UnauthorizedAccessException("Account is deactivated");

            var newAccessToken = _jwtService.GenerateToken(user.Id, user.Email, user.Roles);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            storedToken.MarkAsUsed(newRefreshToken);

            var refreshTokenEntity = RefreshToken.Create(
                user.Id,
                newRefreshToken,
                _jwtService.GetRefreshTokenExpiration()
            );

            await _refreshTokenRepository.AddAsync(refreshTokenEntity, ct);
            await _refreshTokenRepository.SaveChangesAsync(ct);

            _logger.LogInformation("Tokens refreshed for user: {UserId}", user.Id);

            return new LoginUserResponse(user.Id, user.Username, user.Email, newAccessToken, newRefreshToken);
        }
    }
}
