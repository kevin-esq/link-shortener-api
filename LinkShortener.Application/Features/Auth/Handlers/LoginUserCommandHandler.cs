using LinkShortener.Application.Abstractions;
using LinkShortener.Application.Abstractions.Security;
using LinkShortener.Application.Features.Auth.Commands;
using LinkShortener.Application.Features.Auth.DTOs;
using System.Security.Cryptography;
using MediatR;
using System.Text;

namespace LinkShortener.Application.Features.Auth.Handlers
{
    /// <summary>
    /// Handles user login requests by verifying credentials and generating JWT tokens.
    /// </summary>
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginUserResponse>
    {
        private readonly IUserRepository _repository;
        private readonly IJwtService _tokenService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginUserCommandHandler"/> class.
        /// </summary>
        /// <param name="repository">User repository to fetch user data.</param>
        /// <param name="tokenService">JWT service to generate authentication tokens.</param>
        public LoginUserCommandHandler(IUserRepository repository, IJwtService tokenService)
        {
            _repository = repository;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Handles the login request.
        /// </summary>
        /// <param name="request">The login command containing the email and password.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        /// A <see cref="LoginUserResponse"/> containing user info and a JWT token if login is successful.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the email does not exist or the password is incorrect.
        /// </exception>
        public async Task<LoginUserResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            // Retrieve user by email
            var user = await _repository.GetByEmailAsync(request.Email, cancellationToken);
            if (user == null)
                throw new ArgumentException("Invalid email or password.");

            // Verify password
            var passwordHash = HashPassword(request.Password);
            if (user.PasswordHash != passwordHash)
                throw new ArgumentException("Invalid email or password.");

            // Generate JWT token
            var token = _tokenService.GenerateToken(user.Id, user.Email, user.Roles);

            // Return user info and token
            return new LoginUserResponse(user.Id, user.Username, user.Email, token);
        }

        /// <summary>
        /// Computes the SHA-256 hash of the given password.
        /// </summary>
        /// <param name="password">The plain-text password.</param>
        /// <returns>The Base64-encoded SHA-256 hash of the password.</returns>
        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
