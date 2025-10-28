using LinkShortener.Domain.Entities;
using System.Security.Claims;

namespace LinkShortener.Application.Abstractions.Security
{
    public interface IJwtService
    {
        string GenerateToken(Guid userId, string? email = null, IReadOnlyCollection<Role>? roles = null);

        string GenerateRefreshToken();

        IEnumerable<Claim> BuildUserClaims(Guid userId, string? email = null, IEnumerable<string>? roles = null);

        DateTime GetTokenExpiration();

        DateTime GetRefreshTokenExpiration();
    }
}
