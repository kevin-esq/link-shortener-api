using LinkShortener.Application.Abstractions.Services;
using LinkShortener.Application.Features.Auth.Commands;
using MediatR;

namespace LinkShortener.Application.Features.Auth.Handlers
{
    public class VerifyForgotPasswordCodeCommandHandler : IRequestHandler<VerifyForgotPasswordCodeCommand, bool>
    {
        private readonly IVerificationCodeStore _codeStore;

        public VerifyForgotPasswordCodeCommandHandler(IVerificationCodeStore codeStore)
        {
            _codeStore = codeStore;
        }

        public async Task<bool> Handle(VerifyForgotPasswordCodeCommand request, CancellationToken cancellationToken)
        {
            var storedCode = await _codeStore.GetCodeAsync(request.Email, cancellationToken);
            if (string.IsNullOrEmpty(storedCode))
                throw new ArgumentException("Verification code expired or not found.");

            if (!string.Equals(storedCode, request.Code, StringComparison.Ordinal))
                throw new ArgumentException("Invalid verification code.");

            return true;
        }
    }
}
