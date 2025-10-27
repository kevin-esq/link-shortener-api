using LinkShortener.Application.Abstractions;
using LinkShortener.Application.Abstractions.Services;
using LinkShortener.Application.Features.Auth.Commands;
using MediatR;
using System.Security.Cryptography;

namespace LinkShortener.Application.Features.Auth.Handlers
{
    /// <summary>
    /// Handles sending a verification code for the forgot password process.
    /// </summary>
    public class SendForgotPasswordCodeCommandHandler : IRequestHandler<SendForgotPasswordCodeCommand>
    {
        private readonly IUserRepository _repository;
        private readonly IEmailService _emailService;
        private readonly IVerificationCodeStore _codeStore;

        public SendForgotPasswordCodeCommandHandler(
            IUserRepository repository,
            IEmailService emailService,
            IVerificationCodeStore codeStore)
        {
            _repository = repository;
            _emailService = emailService;
            _codeStore = codeStore;
        }

        public async Task Handle(SendForgotPasswordCodeCommand request, CancellationToken cancellationToken)
        {
            var user = await _repository.GetByEmailAsync(request.Email, cancellationToken);
            if (user == null)
                throw new ArgumentException("No user found with the provided email.");

            var code = GenerateCode();

            await _codeStore.SaveCodeAsync(request.Email, code, TimeSpan.FromMinutes(10), cancellationToken);

            var htmlBody = await LoadEmailTemplateAsync(code);

            await _emailService.SendAsync(
                to: request.Email,
                subject: "Password Reset Code - Link Shortener",
                body: htmlBody,
                isHtml: true
            );
        }

        private static string GenerateCode()
        {
            var bytes = new byte[4];
            RandomNumberGenerator.Fill(bytes);
            var num = BitConverter.ToUInt32(bytes, 0) % 1000000;
            return num.ToString("D6");
        }

        private static async Task<string> LoadEmailTemplateAsync(string code)
        {
            var templatePath = Path.Combine(AppContext.BaseDirectory, "HTMLTemplates", "ForgotPasswordEmailTemplate.html");
            var template = await File.ReadAllTextAsync(templatePath);
            return template.Replace("{{CODE}}", code);
        }
    }
}