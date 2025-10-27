namespace LinkShortener.Application.Features.Auth.DTOs
{
    public class VerifyForgotPasswordCodeRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}
