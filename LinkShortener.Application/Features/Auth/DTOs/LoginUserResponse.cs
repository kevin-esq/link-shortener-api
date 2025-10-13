namespace LinkShortener.Application.Features.Auth.DTOs
{
    public record LoginUserResponse(Guid UserId, string Username, string Email, string Token);
}
