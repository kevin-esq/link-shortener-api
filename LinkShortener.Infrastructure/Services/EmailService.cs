using LinkShortener.Application.Abstractions.Services;
using System.Net;
using System.Net.Mail;

namespace LinkShortener.Infrastructure.Services
{
    /// <summary>
    /// Provides functionality to send emails such as verification or password reset codes.
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;
        private readonly string _fromAddress;

        /// <summary>
        /// Initializes a new instance of <see cref="EmailService"/>.
        /// </summary>
        /// <param name="host">SMTP host address.</param>
        /// <param name="port">SMTP port.</param>
        /// <param name="username">SMTP username (usually an email address).</param>
        /// <param name="password">SMTP password or app-specific token.</param>
        /// <param name="fromAddress">The address from which emails will be sent.</param>
        public EmailService(string host, int port, string username, string password, string fromAddress)
        {
            _fromAddress = fromAddress;
            _smtpClient = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true
            };
        }

        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name="to">Recipient email address.</param>
        /// <param name="subject">Email subject line.</param>
        /// <param name="body">Email message body (plain text or HTML).</param>
        /// <param name="isHtml">Indicates whether the body contains HTML content.</param>
        public async Task SendAsync(string to, string subject, string body, bool isHtml = false)
        {
            using var message = new MailMessage(_fromAddress, to)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            await _smtpClient.SendMailAsync(message);
        }
    }
}