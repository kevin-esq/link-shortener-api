using LinkShortener.Application.Abstractions;
using LinkShortener.Application.Abstractions.Security;
using LinkShortener.Application.Abstractions.Services;
using LinkShortener.Application.Common.Validators;
using LinkShortener.Application.Features.Auth.Commands;
using LinkShortener.Application.Features.Auth.DTOs;
using LinkShortener.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace LinkShortener.Application.Features.Auth.Handlers
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
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

        public async Task<RegisterUserResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
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

                var emailBody = BuildVerificationEmailBody(user.Username, verificationCode);

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

        private static string BuildVerificationEmailBody(string username, string code)
        {
            return $@"
                <html>
                <body style='font-family: Arial, sans-serif; padding: 20px; background-color: #f5f5f5;'>
                    <div style='max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);'>
                        <h2 style='color: #333; margin-bottom: 20px;'>Welcome to LinkShortener, {username}!</h2>
                        <p style='color: #666; font-size: 16px; line-height: 1.5;'>
                            Thank you for registering. To complete your registration and start using your account, 
                            please verify your email address using the code below:
                        </p>
                        <div style='background-color: #f0f7ff; padding: 20px; border-radius: 5px; text-align: center; margin: 25px 0;'>
                            <p style='margin: 0; font-size: 14px; color: #666; margin-bottom: 10px;'>Your verification code is:</p>
                            <p style='margin: 0; font-size: 32px; font-weight: bold; color: #007bff; letter-spacing: 5px;'>{code}</p>
                        </div>
                        <p style='color: #999; font-size: 14px; margin-top: 20px;'>
                            This code will expire in {CODE_EXPIRATION_MINUTES} minutes.
                        </p>
                        <p style='color: #999; font-size: 12px; margin-top: 30px; padding-top: 20px; border-top: 1px solid #eee;'>
                            If you didn't create an account with LinkShortener, please ignore this email.
                        </p>
                    </div>
                </body>
                </html>
            ";
        }
    }
}
