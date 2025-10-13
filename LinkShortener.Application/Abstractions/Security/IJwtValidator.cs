namespace LinkShortener.Application.Abstractions.Security
{
    public interface IJwtValidator
    {
        bool ValidateToken(string token);
        Guid? GetUserIdFromToken(string token);
    }
}
