namespace LinkShortener.Application.Features.Auth.DTOs
{
    public class VerifyEmailCodeRequest
    {
        public string Email { get; set; } = default!;
        public string Code { get; set; } = default!;
    }
}
