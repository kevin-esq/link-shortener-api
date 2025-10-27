using LinkShortener.Application.Abstractions;
using LinkShortener.Application.Abstractions.Services;
using LinkShortener.Application.Features.Auth.Commands;
using MediatR;
using System.Security.Cryptography;
using System.Text;

namespace LinkShortener.Application.Features.Auth.Handlers
{
    /// <summary>
    /// Handles password reset requests after verifying the code.
    /// </summary>
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
    {
        private readonly IUserRepository _repository;
        private readonly IVerificationCodeStore _codeStore;

        public ResetPasswordCommandHandler(IUserRepository repository, IVerificationCodeStore codeStore)
        {
            _repository = repository;
            _codeStore = codeStore;
        }

        public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var storedCode = await _codeStore.GetCodeAsync(request.Email, cancellationToken);
            if (string.IsNullOrEmpty(storedCode) || storedCode != request.Code)
                throw new ArgumentException("Invalid or expired verification code.");

            var user = await _repository.GetByEmailAsync(request.Email, cancellationToken);
            if (user == null)
                throw new ArgumentException("User not found.");

            var newHash = HashPassword(request.NewPassword);

            user.ChangePassword(newHash);
            await _repository.SaveChangesAsync(cancellationToken);

            await _codeStore.DeleteCodeAsync(request.Email, cancellationToken);
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
