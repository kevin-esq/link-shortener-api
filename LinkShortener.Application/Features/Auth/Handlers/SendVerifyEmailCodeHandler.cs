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

                var body = await LoadEmailTemplateAsync(code);

                await _emailService.SendAsync(
                    request.Email,
                    "Verify your email - Link Shortener",
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

        private static async Task<string> LoadEmailTemplateAsync(string code)
        {
            var templatePath = Path.Combine(AppContext.BaseDirectory, "HTMLTemplates", "VerifyEmailTemplate.html");
            var template = await File.ReadAllTextAsync(templatePath);
            return template.Replace("{{CODE}}", code);
        }
    }
}