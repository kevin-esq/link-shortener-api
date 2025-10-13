using LinkShortener.Application.Abstractions;
using LinkShortener.Application.Features.Auth.Commands;
using LinkShortener.Application.Features.Auth.DTOs;
using LinkShortener.Domain.Entities;
using MediatR;
using System.Security.Cryptography;
using System.Text;

namespace LinkShortener.Application.Features.Auth.Handlers
{
    /// <summary>
    /// Handles user registration requests by creating new users and storing their hashed passwords.
    /// </summary>
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
    {
        private readonly IUserRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterUserCommandHandler"/> class.
        /// </summary>
        /// <param name="repository">The user repository used to persist user data.</param>
        public RegisterUserCommandHandler(IUserRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Handles the registration request.
        /// </summary>
        /// <param name="request">The registration command containing username, email, and password.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        /// A <see cref="RegisterUserResponse"/> containing the newly registered user's ID, username, and email.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown if the email is already registered.
        /// </exception>
        public async Task<RegisterUserResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // Check if the email is already registered
            if (await _repository.ExistsByEmailAsync(request.Email, cancellationToken))
                throw new ArgumentException("Email already registered.");

            // Hash the password
            var passwordHash = HashPassword(request.Password);

            // Create the new user entity
            var user = User.Create(request.Username, request.Email, passwordHash);

            // Persist the user
            await _repository.AddAsync(user, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            // Return response with user info
            return new RegisterUserResponse(user.Id, user.Username, user.Email);
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
