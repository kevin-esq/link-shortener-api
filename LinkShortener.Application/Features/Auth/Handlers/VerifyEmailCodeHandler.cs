using LinkShortener.Application.Abstractions;
using LinkShortener.Application.Abstractions.Services;
using LinkShortener.Application.Features.Auth.Commands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LinkShortener.Application.Features.Auth.Handlers
{
    public class VerifyEmailCodeHandler : IRequestHandler<VerifyEmailCodeCommand, bool>
    {
        private readonly IVerificationCodeStore _codeStore;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<VerifyEmailCodeHandler> _logger;

        public VerifyEmailCodeHandler(
            IVerificationCodeStore codeStore,
            IUserRepository userRepository,
            ILogger<VerifyEmailCodeHandler> logger)
        {
            _codeStore = codeStore;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(VerifyEmailCodeCommand request, CancellationToken ct)
        {
            var storedCode = await _codeStore.GetCodeAsync(request.Email, ct);
            if (storedCode == null || storedCode != request.Code)
            {
                _logger.LogWarning("Invalid or expired verification code for email: {Email}", request.Email);
                return false;
            }

            var user = await _userRepository.GetByEmailAsync(request.Email, ct);
            if (user == null)
            {
                _logger.LogWarning("User not found for email: {Email}", request.Email);
                await _codeStore.DeleteCodeAsync(request.Email, ct);
                return false;
            }

            user.VerifyEmail();
            await _userRepository.SaveChangesAsync(ct);
            await _codeStore.DeleteCodeAsync(request.Email, ct);

            _logger.LogInformation("Email verified successfully for user: {UserId}", user.Id);
            return true;
        }
    }
}
