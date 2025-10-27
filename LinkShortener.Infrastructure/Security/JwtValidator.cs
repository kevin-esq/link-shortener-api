using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using LinkShortener.Application.Abstractions.Security;

namespace LinkShortener.Infrastructure.Security
{
    public class JwtValidator(TokenValidationParameters validationParameters) : IJwtValidator
    {
        private readonly TokenValidationParameters _validationParameters = validationParameters;

        public bool ValidateToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            try
            {
                handler.ValidateToken(token, _validationParameters, out _);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Guid? GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var sub = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(sub, out var userId))
                return userId;

            return null;
        }
    }
}
