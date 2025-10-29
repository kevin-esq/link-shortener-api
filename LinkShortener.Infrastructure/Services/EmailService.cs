using LinkShortener.Application.Abstractions.Services;
using Microsoft.Extensions.Logging;
using Resend;

namespace LinkShortener.Infrastructure.Services
{
    /// <summary>
    /// Provides functionality to send emails using Resend API.
    /// Falls back to console logging if Resend API key is not configured.
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly IResend? _resend;
        private readonly string _fromAddress;
        private readonly ILogger<EmailService> _logger;
        private readonly bool _isConfigured;

        /// <summary>
        /// Initializes a new instance of <see cref="EmailService"/>.
        /// </summary>
        /// <param name="resendApiKey">Resend API key (starts with 're_').</param>
        /// <param name="fromAddress">The email address from which emails will be sent.</param>
        /// <param name="logger">Logger instance.</param>
        public EmailService(string? resendApiKey, string fromAddress, ILogger<EmailService> logger)
        {
            _fromAddress = fromAddress;
            _logger = logger;

            // Check if Resend API is properly configured
            _isConfigured = !string.IsNullOrWhiteSpace(resendApiKey);

            if (_isConfigured)
            {
                try
                {
                    _resend = ResendClient.Create(resendApiKey!);
                    _logger.LogInformation("✅ Resend email client initialized successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to initialize Resend client. Email logging will be used instead.");
                    _isConfigured = false;
                }
            }
            else
            {
                _logger.LogWarning("⚠️ Resend API key not configured. Emails will be logged to console instead.");
            }
        }

        /// <summary>
        /// Sends an email asynchronously or logs it if Resend API is not configured.
        /// </summary>
        /// <param name="to">Recipient email address.</param>
        /// <param name="subject">Email subject line.</param>
        /// <param name="body">Email message body (plain text or HTML).</param>
        /// <param name="isHtml">Indicates whether the body contains HTML content.</param>
        public async Task SendAsync(string to, string subject, string body, bool isHtml = false)
        {
            if (!_isConfigured || _resend == null)
            {
                // Log email instead of sending
                _logger.LogWarning(
                    "📧 EMAIL NOT SENT (Resend API not configured)\n" +
                    "To: {To}\n" +
                    "From: {From}\n" +
                    "Subject: {Subject}\n" +
                    "Body: {Body}",
                    to, _fromAddress, subject, body
                );
                return;
            }

            try
            {
                var message = new EmailMessage
                {
                    From = _fromAddress,
                    To = to,
                    Subject = subject,
                    HtmlBody = isHtml ? body : null,
                    TextBody = !isHtml ? body : null
                };

                var response = await _resend.EmailSendAsync(message);
                _logger.LogInformation("✅ Email sent successfully to {To} via Resend", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Unexpected error sending email to {To}", to);
                throw;
            }
        }
    }
}