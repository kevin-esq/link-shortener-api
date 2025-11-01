using LinkShortener.Application.Abstractions;
using LinkShortener.Application.Abstractions.Security;
using LinkShortener.Application.Abstractions.Services;
using LinkShortener.Application.Common.Validators;
using LinkShortener.Application.Features.Auth.Commands;
using LinkShortener.Application.Features.Auth.DTOs;
using LinkShortener.Domain.Entities;
using LiteBus.Commands.Abstractions;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace LinkShortener.Application.Features.Auth.Handlers
{
    public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, RegisterUserResponse>
    {
        private readonly IUserRepository _repository;
        private readonly IEmailService _emailService;
        private readonly IVerificationCodeStore _codeStore;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<RegisterUserCommandHandler> _logger;
        private const int CODE_EXPIRATION_MINUTES = 10;
        private const int CODE_MIN = 100000;
        private const int CODE_MAX = 999999;

        public RegisterUserCommandHandler(
            IUserRepository repository,
            IEmailService emailService,
            IVerificationCodeStore codeStore,
            IPasswordHasher passwordHasher,
            ILogger<RegisterUserCommandHandler> logger)
        {
            _repository = repository;
            _emailService = emailService;
            _codeStore = codeStore;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<RegisterUserResponse> HandleAsync(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            if (await _repository.ExistsByEmailAsync(request.Email, cancellationToken))
                throw new ArgumentException("Email already registered");

            var (isValid, errorMessage) = PasswordValidator.Validate(request.Password);
            if (!isValid)
                throw new ArgumentException(errorMessage);

            var passwordHash = _passwordHasher.HashPassword(request.Password);
            var user = User.Create(request.Username, request.Email, passwordHash);

            await _repository.AddAsync(user, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            try
            {
                var verificationCode = GenerateSecureCode();

                await _codeStore.SaveCodeAsync(
                    user.Email,
                    verificationCode,
                    TimeSpan.FromMinutes(CODE_EXPIRATION_MINUTES),
                    cancellationToken);

                var emailBody = BuildVerificationEmailBody(verificationCode);

                await _emailService.SendAsync(
                    user.Email,
                    "Verify your email - LinkShortener",
                    emailBody,
                    isHtml: true);

                _logger.LogInformation(
                    "User {UserId} registered successfully. Verification code sent to {Email}",
                    user.Id,
                    user.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to send verification email to {Email} after registration",
                    user.Email);
            }

            return new RegisterUserResponse(user.Id, user.Username, user.Email, user.IsEmailVerified);
        }


        private static string GenerateSecureCode()
        {
            var number = RandomNumberGenerator.GetInt32(CODE_MIN, CODE_MAX + 1);
            return number.ToString();
        }

        private static string BuildVerificationEmailBody(string code)
        {
            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HTMLTemplates", "VerifyEmailTemplate.html");
            var template = File.ReadAllText(templatePath);
            return template.Replace("{{CODE}}", code);
        }
    }
}
