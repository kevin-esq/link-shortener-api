using System.Security.Cryptography;
using LinkShortener.Application.Abstractions;
using LinkShortener.Application.Abstractions.Services;
using LinkShortener.Application.Features.Auth.Commands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LinkShortener.Application.Features.Auth.Handlers
{
    public class SendVerifyEmailCodeHandler : IRequestHandler<SendVerifyEmailCodeCommand>
    {
        private readonly IEmailService _emailService;
        private readonly IVerificationCodeStore _codeStore;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<SendVerifyEmailCodeHandler> _logger;
        private const int CODE_EXPIRATION_MINUTES = 10;
        private const int CODE_MIN = 100000;
        private const int CODE_MAX = 999999;

        public SendVerifyEmailCodeHandler(
            IEmailService emailService,
            IVerificationCodeStore codeStore,
            IUserRepository userRepository,
            ILogger<SendVerifyEmailCodeHandler> logger)
        {
            _emailService = emailService;
            _codeStore = codeStore;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task Handle(SendVerifyEmailCodeCommand request, CancellationToken ct)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(request.Email, ct);
                if (user == null)
                {
                    _logger.LogWarning("Verification code requested for non-existent email: {Email}", request.Email);
                    throw new ArgumentException("User not found with the provided email");
                }

                if (user.IsEmailVerified)
                {
                    _logger.LogInformation("Verification code requested for already verified email: {Email}", request.Email);
                    throw new InvalidOperationException("Email is already verified");
                }

                var code = GenerateSecureCode();

                await _codeStore.SaveCodeAsync(
                    request.Email,
                    code,
                    TimeSpan.FromMinutes(CODE_EXPIRATION_MINUTES),
                    ct);

                var body = BuildEmailBody(code);

                await _emailService.SendAsync(
                    request.Email,
                    "Verify your email",
                    body,
                    isHtml: true
                    );

                _logger.LogInformation(
                    "Verification code sent successfully to {Email}",
                    request.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to send verification code to {Email}",
                    request.Email);
                throw;
            }
        }

        private static string GenerateSecureCode()
        {
            var number = RandomNumberGenerator.GetInt32(CODE_MIN, CODE_MAX + 1);
            return number.ToString();
        }

        private static string BuildEmailBody(string code)
        {
            return $@"
                <html>
                <body style='font-family: Arial, sans-serif; padding: 20px; background-color: #f5f5f5;'>
                    <div style='max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);'>
                        <h2 style='color: #333; margin-bottom: 20px;'>Email Verification</h2>
                        <p style='color: #666; font-size: 16px; line-height: 1.5;'>
                            You requested a verification code for your LinkShortener account.
                            Please use the code below to verify your email:
                        </p>
                        <div style='background-color: #f0f7ff; padding: 20px; border-radius: 5px; text-align: center; margin: 25px 0;'>
                            <p style='margin: 0; font-size: 14px; color: #666; margin-bottom: 10px;'>Your verification code is:</p>
                            <p style='margin: 0; font-size: 32px; font-weight: bold; color: #007bff; letter-spacing: 5px;'>{code}</p>
                        </div>
                        <p style='color: #999; font-size: 14px; margin-top: 20px;'>
                            This code will expire in {CODE_EXPIRATION_MINUTES} minutes.
                        </p>
                        <p style='color: #999; font-size: 12px; margin-top: 30px; padding-top: 20px; border-top: 1px solid #eee;'>
                            If you didn't request this code, please ignore this email.
                        </p>
                    </div>
                </body>
                </html>
            ";
        }
    }
}