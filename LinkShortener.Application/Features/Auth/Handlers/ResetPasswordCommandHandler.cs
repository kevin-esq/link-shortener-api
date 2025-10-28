using LinkShortener.Application.Abstractions;
using LinkShortener.Application.Abstractions.Security;
using LinkShortener.Application.Abstractions.Services;
using LinkShortener.Application.Common.Validators;
using LinkShortener.Application.Features.Auth.Commands;
using MediatR;

namespace LinkShortener.Application.Features.Auth.Handlers
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
    {
        private readonly IUserRepository _repository;
        private readonly IVerificationCodeStore _codeStore;
        private readonly IPasswordHasher _passwordHasher;

        public ResetPasswordCommandHandler(
            IUserRepository repository,
            IVerificationCodeStore codeStore,
            IPasswordHasher passwordHasher)
        {
            _repository = repository;
            _codeStore = codeStore;
            _passwordHasher = passwordHasher;
        }

        public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var storedCode = await _codeStore.GetCodeAsync(request.Email, cancellationToken);
            if (string.IsNullOrEmpty(storedCode) || storedCode != request.Code)
                throw new ArgumentException("Invalid or expired verification code.");

            var user = await _repository.GetByEmailAsync(request.Email, cancellationToken);
            if (user == null)
                throw new ArgumentException("User not found.");

            var (isValid, errorMessage) = PasswordValidator.Validate(request.NewPassword);
            if (!isValid)
                throw new ArgumentException(errorMessage);

            var newHash = _passwordHasher.HashPassword(request.NewPassword);

            user.ChangePassword(newHash);
            await _repository.SaveChangesAsync(cancellationToken);

            await _codeStore.DeleteCodeAsync(request.Email, cancellationToken);
        }

    }
}
