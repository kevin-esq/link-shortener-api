using LinkShortener.Application.Abstractions.Security;
using LinkShortener.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LinkShortener.Infrastructure.Security
{
    /// <summary>
    /// Service responsible for generating JSON Web Tokens (JWT) for authenticated users.
    /// </summary>
    /// <remarks>
    /// This class implements <see cref="IJwtService"/> and provides functionality for creating
    /// secure, signed JWTs using asymmetric RSA encryption. It includes user claims such as
    /// identifiers, email, and roles to support role-based authentication and authorization.
    /// </remarks>
    public class JwtService : IJwtService
    {
        private readonly RsaSecurityKey _privateKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly TimeSpan _defaultLifetime = TimeSpan.FromHours(1);

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtService"/> class.
        /// </summary>
        /// <param name="privateKey">The RSA private key used to sign the generated tokens.</param>
        /// <param name="issuer">The entity (application or server) issuing the token.</param>
        /// <param name="audience">The intended audience that should accept the token.</param>
        public JwtService(RsaSecurityKey privateKey, string issuer, string audience)
        {
            _privateKey = privateKey;
            _issuer = issuer;
            _audience = audience;
        }

        /// <summary>
        /// Generates a signed JWT for the specified user.
        /// </summary>
        /// <param name="userId">The unique identifier (GUID) of the user.</param>
        /// <param name="email">Optional user email claim.</param>
        /// <param name="roles">Optional collection of user roles to embed as claims.</param>
        /// <returns>
        /// A compact serialized JWT string containing the user claims, issuer, audience, and expiration.
        /// </returns>
        /// <remarks>
        /// - The token is valid for 1 hour by default.<br/>
        /// - Uses RSA SHA-256 for signing to ensure asymmetric key security.<br/>
        /// - Includes standard JWT claims like <c>sub</c>, <c>jti</c>, and <c>nameidentifier</c>.
        /// </remarks>
        public string GenerateToken(Guid userId, string? email = null, IReadOnlyCollection<UserRole>? roles = null)
        {
            var now = DateTime.UtcNow;
            var expires = now.Add(_defaultLifetime);

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

        /// <summary>
        /// Builds a list of standard and custom user claims to include in the JWT.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="email">Optional email address of the user.</param>
        /// <param name="roles">Optional set of user roles to include as <see cref="ClaimTypes.Role"/> claims.</param>
        /// <returns>A collection of <see cref="Claim"/> objects representing the user's identity.</returns>
        /// <remarks>
        /// Adds the following standard claims:
        /// <list type="bullet">
        ///   <item><description><c>sub</c> – User ID (subject)</description></item>
        ///   <item><description><c>nameidentifier</c> – Duplicate of user ID for compatibility</description></item>
        ///   <item><description><c>jti</c> – Unique token identifier</description></item>
        /// </list>
        /// Includes <c>email</c> and <c>role</c> claims when provided.
        /// </remarks>
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

        /// <summary>
        /// Returns the expiration date and time for a newly generated token.
        /// </summary>
        /// <returns>A <see cref="DateTime"/> representing when a new token would expire.</returns>
        /// <remarks>
        /// The expiration is always relative to the current UTC time plus the default lifetime (1 hour).
        /// </remarks>
        public DateTime GetTokenExpiration() =>
            DateTime.UtcNow.Add(_defaultLifetime);
    }
}
