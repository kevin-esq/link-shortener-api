namespace LinkShortener.Application.Features.Auth.DTOs
{
    public record RegisterUserResponse(Guid UserId, string Username, string Email, bool IsEmailVerified);
}
