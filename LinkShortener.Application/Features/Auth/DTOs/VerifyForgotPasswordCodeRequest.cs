namespace LinkShortener.Application.Features.Auth.DTOs
{
    /// <summary>
    /// Represents the request body for verifying a forgot password code.
    /// </summary>
    public class VerifyForgotPasswordCodeRequest
    {
        /// <summary>
        /// The email address associated with the account.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// The verification code sent to the user's email.
        /// </summary>
        public string Code { get; set; } = string.Empty;
    }
}
