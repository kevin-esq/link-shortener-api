namespace LinkShortener.Application.Features.Auth.DTOs
{
    public class SendVerifyEmailCodeRequest
    {
        public string Email { get; set; } = default!;
    }
}
