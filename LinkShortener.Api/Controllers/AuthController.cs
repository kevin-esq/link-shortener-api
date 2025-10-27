using LinkShortener.Application.Features.Auth.Commands;
using LinkShortener.Application.Features.Auth.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace LinkShortener.Api.Controllers
{
    /// <summary>
    /// Handles user authentication and registration for the Link Shortener API.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        /// <summary>
        /// Registers a new user account.
        /// </summary>
        /// <remarks>
        /// Creates a new user by providing a username, email, and password.  
        /// Returns basic user information and an authentication token upon success.
        ///
        /// **Example request:**
        /// ```json
        /// {
        ///   "username": "kevin123",
        ///   "email": "kevin@example.com",
        ///   "password": "StrongP@ssw0rd!"
        /// }
        /// ```
        ///
        /// **Example successful response:**
        /// ```json
        /// {
        ///   "id": "e91e89d2-77b0-4a8c-b6dc-5f4e9d79d32a",
        ///   "username": "kevin123",
        ///   "email": "kevin@example.com",
        ///   "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
        /// }
        /// ```
        /// </remarks>
        /// <param name="request">The registration details (username, email, password).</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <response code="200">Returns the newly created user and JWT token.</response>
        /// <response code="400">If the request data is invalid or user already exists.</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(RegisterUserResponse), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new RegisterUserCommand(
                request.Username,
                request.Email,
                request.Password),
                cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// </summary>
        /// <remarks>
        /// This endpoint verifies user credentials and returns an authentication token used for subsequent requests.  
        /// 
        /// **Example request:**
        /// ```json
        /// {
        ///   "email": "kevin@example.com",
        ///   "password": "StrongP@ssw0rd!"
        /// }
        /// ```
        ///
        /// **Example successful response:**
        /// ```json
        /// {
        ///   "id": "e91e89d2-77b0-4a8c-b6dc-5f4e9d79d32a",
        ///   "username": "kevin123",
        ///   "email": "kevin@example.com",
        ///   "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
        /// }
        /// ```
        /// </remarks>
        /// <param name="request">The login credentials (email and password).</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <response code="200">Returns user information and JWT token.</response>
        /// <response code="400">If the credentials are invalid.</response>
        /// <response code="401">If the authentication fails.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginUserResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new LoginUserCommand(
                request.Email,
                request.Password),
                cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Sends a password reset code to the user's email.
        /// </summary>
        /// <param name="email">Registered user email.</param>
        /// <response code="200">If the code was sent successfully.</response>
        /// <response code="404">If the email is not registered.</response>
        [HttpGet("forgot-password/code")]
        public async Task<IActionResult> SendForgotPasswordCode([FromQuery] string email, CancellationToken cancellationToken)
        {
            await _mediator.Send(new SendForgotPasswordCodeCommand(email), cancellationToken);
            return Ok(new { message = "Verification code sent to email." });
        }

        // 🔹 2️⃣ Verificar código
        /// <summary>
        /// Verifies the password reset code sent to the user's email.
        /// </summary>
        /// <param name="request">Contains email and code.</param>
        /// <response code="200">If the code is valid.</response>
        /// <response code="400">If the code is invalid or expired.</response>
        [HttpPost("forgot-password/verify")]
        public async Task<IActionResult> VerifyForgotPasswordCode([FromBody] VerifyForgotPasswordCodeRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new VerifyForgotPasswordCodeCommand(request.Email, request.Code), cancellationToken);
            return Ok(result);
        }

        // 🔹 3️⃣ Resetear contraseña
        /// <summary>
        /// Resets the user's password after successful code verification.
        /// </summary>
        /// <param name="request">Contains email, code, and new password.</param>
        /// <response code="200">If the password was successfully reset.</response>
        /// <response code="400">If the code is invalid or expired.</response>
        [HttpPost("forgot-password/reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
        {
            await _mediator.Send(new ResetPasswordCommand(request.Email, request.ResetCode, request.NewPassword), cancellationToken);
            return Ok(new { message = "Password successfully reset." });
        }
    }
}
