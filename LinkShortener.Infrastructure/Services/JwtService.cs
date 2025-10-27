using LinkShortener.Application.Abstractions.Security;
using LinkShortener.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace LinkShortener.Infrastructure.Security
{
    public class JwtService : IJwtService
    {
        private readonly RsaSecurityKey _privateKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly TimeSpan _accessTokenLifetime = TimeSpan.FromMinutes(15);
        private readonly TimeSpan _refreshTokenLifetime = TimeSpan.FromDays(7);

        public JwtService(RsaSecurityKey privateKey, string issuer, string audience)
        {
            _privateKey = privateKey;
            _issuer = issuer;
            _audience = audience;
        }

        public string GenerateToken(Guid userId, string? email = null, IReadOnlyCollection<UserRole>? roles = null)
        {
            var now = DateTime.UtcNow;
            var expires = now.Add(_accessTokenLifetime);

            var claims = BuildUserClaims(userId, email, roles?.Select(r => r.ToString()));
            var credentials = new SigningCredentials(_privateKey, SecurityAlgorithms.RsaSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public IEnumerable<Claim> BuildUserClaims(Guid userId, string? email = null, IEnumerable<string>? roles = null)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (!string.IsNullOrWhiteSpace(email))
                claims.Add(new Claim(ClaimTypes.Email, email));

            if (roles != null)
                claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            return claims;
        }

        public DateTime GetTokenExpiration() => DateTime.UtcNow.Add(_accessTokenLifetime);

        public DateTime GetRefreshTokenExpiration() => DateTime.UtcNow.Add(_refreshTokenLifetime);
    }
}
