using LinkShortener.Application.Common.Models;
using LinkShortener.Application.Features.Auth.Commands;
using LinkShortener.Application.Features.Auth.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace LinkShortener.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        /// <summary>
        /// Registers a new user account and sends email verification code.
        /// </summary>
        /// <remarks>
        /// Creates a new user by providing a username, email, and password.  
        /// A verification code will be automatically sent to the provided email address.
        /// **You must verify your email before you can log in.**
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
        ///   "success": true,
        ///   "data": {
        ///     "userId": "e91e89d2-77b0-4a8c-b6dc-5f4e9d79d32a",
        ///     "username": "kevin123",
        ///     "email": "kevin@example.com",
        ///     "isEmailVerified": false
        ///   },
        ///   "message": "Registration successful. Please check your email for verification code."
        /// }
        /// ```
        /// 
        /// **Next steps:**
        /// 1. Check your email inbox for the 6-digit verification code
        /// 2. Use the `/api/auth/verify-email/confirm` endpoint to verify your email
        /// 3. Once verified, you can log in using `/api/auth/login`
        /// </remarks>
        /// <param name="request">The registration details (username, email, password).</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <response code="200">Returns the newly created user information. Email verification required to login.</response>
        /// <response code="400">If the request data is invalid or user already exists.</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<RegisterUserResponse>), 200)]
        [ProducesResponseType(typeof(ErrorDetails), 400)]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new RegisterUserCommand(
                request.Username,
                request.Email,
                request.Password),
                cancellationToken);

            return Ok(ApiResponse<RegisterUserResponse>.SuccessResponse(
                result,
                "Registration successful. Please check your email for verification code."));
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// </summary>
        /// <remarks>
        /// This endpoint verifies user credentials and returns an authentication token used for subsequent requests.  
        /// **IMPORTANT: Your email must be verified before you can log in.**
        /// If you haven't verified your email yet, use the `/api/auth/verify-email/code` endpoint to request a new verification code.
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
        ///   "success": true,
        ///   "data": {
        ///     "id": "e91e89d2-77b0-4a8c-b6dc-5f4e9d79d32a",
        ///     "username": "kevin123",
        ///     "email": "kevin@example.com",
        ///     "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
        ///   },
        ///   "message": "Login successful"
        /// }
        /// ```
        /// </remarks>
        /// <param name="request">The login credentials (email and password).</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <response code="200">Returns user information and JWT token.</response>
        /// <response code="400">If the credentials are invalid.</response>
        /// <response code="401">If the authentication fails or email is not verified.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<LoginUserResponse>), 200)]
        [ProducesResponseType(typeof(ErrorDetails), 400)]
        [ProducesResponseType(typeof(ErrorDetails), 401)]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new LoginUserCommand(
                request.Email,
                request.Password),
                cancellationToken);

            return Ok(ApiResponse<LoginUserResponse>.SuccessResponse(result, "Login successful"));
        }

        /// <summary>
        /// Sends a password reset code to the user's email.
        /// </summary>
        /// <param name="email">Registered user email.</param>
        /// <response code="200">If the code was sent successfully.</response>
        /// <response code="404">If the email is not registered.</response>
        [HttpGet("forgot-password/code")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ErrorDetails), 404)]
        public async Task<IActionResult> SendForgotPasswordCode([FromQuery] string email, CancellationToken cancellationToken)
        {
            await _mediator.Send(new SendForgotPasswordCodeCommand(email), cancellationToken);
            return Ok(ApiResponse.SuccessResponse("Verification code sent to email"));
        }

        /// <summary>
        /// Verifies the password reset code sent to the user's email.
        /// </summary>
        /// <param name="request">Contains email and code.</param>
        /// <response code="200">If the code is valid.</response>
        /// <response code="400">If the code is invalid or expired.</response>
        [HttpPost("forgot-password/verify")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(ErrorDetails), 400)]
        public async Task<IActionResult> VerifyForgotPasswordCode([FromBody] VerifyForgotPasswordCodeRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new VerifyForgotPasswordCodeCommand(request.Email, request.Code), cancellationToken);
            return Ok(ApiResponse<bool>.SuccessResponse(result, "Code verified successfully"));
        }

        /// <summary>
        /// Resets the user's password after successful code verification.
        /// </summary>
        /// <param name="request">Contains email, code, and new password.</param>
        /// <response code="200">If the password was successfully reset.</response>
        /// <response code="400">If the code is invalid or expired.</response>
        [HttpPost("forgot-password/reset")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ErrorDetails), 400)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
        {
            await _mediator.Send(new ResetPasswordCommand(request.Email, request.ResetCode, request.NewPassword), cancellationToken);
            return Ok(ApiResponse.SuccessResponse("Password successfully reset"));
        }

        /// <summary>
        /// Sends or resends a verification code to the user's email.
        /// </summary>
        /// <remarks>
        /// Use this endpoint to request a new verification code if:
        /// - You didn't receive the initial code after registration
        /// - Your previous code expired (codes are valid for 10 minutes)
        /// - You need to verify your email to log in
        /// 
        /// **Note:** This endpoint will only send a code if the email exists and is not already verified.
        /// 
        /// **Example request:**
        /// ```bash
        /// GET /api/auth/verify-email/code?email=kevin@example.com
        /// ```
        /// 
        /// **Example response:**
        /// ```json
        /// {
        ///   "success": true,
        ///   "message": "Verification code sent to email"
        /// }
        /// ```
        /// </remarks>
        /// <param name="email">The email address to send the verification code to.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <response code="200">Verification code sent successfully.</response>
        /// <response code="400">If the email doesn't exist or is already verified.</response>
        [HttpGet("verify-email/code")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ErrorDetails), 400)]
        public async Task<IActionResult> SendVerifyEmailCode([FromQuery] string email, CancellationToken ct)
        {
            await _mediator.Send(new SendVerifyEmailCodeCommand(email), ct);
            return Ok(ApiResponse.SuccessResponse("Verification code sent to email"));
        }

        /// <summary>
        /// Verifies the user's email using the code sent to their inbox.
        /// </summary>
        /// <remarks>
        /// After registering or requesting a verification code, use this endpoint to verify your email address.
        /// Once verified, you will be able to log in to your account.
        /// 
        /// **Example request:**
        /// ```json
        /// {
        ///   "email": "kevin@example.com",
        ///   "code": "123456"
        /// }
        /// ```
        /// 
        /// **Example success response:**
        /// ```json
        /// {
        ///   "success": true,
        ///   "message": "Email successfully verified. You can now log in"
        /// }
        /// ```
        /// </remarks>
        /// <param name="request">The email and verification code.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <response code="200">Email verified successfully.</response>
        /// <response code="400">If the code is invalid or expired.</response>
        [HttpPost("verify-email/confirm")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ErrorDetails), 400)]
        public async Task<IActionResult> VerifyEmailCode([FromBody] VerifyEmailCodeRequest request, CancellationToken ct)
        {
            var isValid = await _mediator.Send(new VerifyEmailCodeCommand(request.Email, request.Code), ct);
            if (!isValid)
                return BadRequest(ApiResponse.ErrorResponse("Invalid or expired code"));

            return Ok(ApiResponse.SuccessResponse("Email successfully verified. You can now log in"));
        }

        /// <summary>
        /// Authenticates or registers a user via Google OAuth.
        /// </summary>
        /// <remarks>
        /// Send the Google ID token obtained from the frontend Google Sign-In.
        /// If the email doesn't exist, a new account will be created automatically.
        /// Google accounts are automatically verified and don't require email confirmation.
        /// 
        /// **Example request:**
        /// ```json
        /// {
        ///   "idToken": "eyJhbGciOiJSUzI1NiIsImtpZCI6IjU5M..."
        /// }
        /// ```
        /// 
        /// **Example response:**
        /// ```json
        /// {
        ///   "success": true,
        ///   "data": {
        ///     "id": "guid",
        ///     "username": "johnsmith123",
        ///     "email": "john@gmail.com",
        ///     "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
        ///   },
        ///   "message": "Google authentication successful"
        /// }
        /// ```
        /// </remarks>
        [HttpPost("google")]
        [ProducesResponseType(typeof(ApiResponse<LoginUserResponse>), 200)]
        [ProducesResponseType(typeof(ErrorDetails), 400)]
        [ProducesResponseType(typeof(ErrorDetails), 401)]
        public async Task<IActionResult> GoogleAuth([FromBody] GoogleAuthRequest request, CancellationToken ct)
        {
            var result = await _mediator.Send(new GoogleAuthCommand(request.IdToken), ct);
            return Ok(ApiResponse<LoginUserResponse>.SuccessResponse(result, "Google authentication successful"));
        }

        /// <summary>
        /// Refreshes an expired access token using a valid refresh token.
        /// </summary>
        /// <remarks>
        /// When your access token expires (after 15 minutes), use this endpoint to get a new one without requiring the user to log in again.
        /// The refresh token is valid for 7 days.
        /// 
        /// **Security:**
        /// - Each refresh token can only be used once
        /// - After use, a new refresh token is issued
        /// - Old refresh token is marked as used
        /// - This prevents token replay attacks
        /// 
        /// **Example request:**
        /// ```json
        /// {
        ///   "refreshToken": "base64_encoded_token_here"
        /// }
        /// ```
        /// 
        /// **Example response:**
        /// ```json
        /// {
        ///   "success": true,
        ///   "data": {
        ///     "userId": "guid",
        ///     "username": "kevin123",
        ///     "email": "kevin@example.com",
        ///     "token": "new_access_token",
        ///     "refreshToken": "new_refresh_token"
        ///   },
        ///   "message": "Tokens refreshed successfully"
        /// }
        /// ```
        /// </remarks>
        /// <param name="request">The refresh token.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <response code="200">New tokens issued successfully.</response>
        /// <response code="401">If the refresh token is invalid, expired, revoked, or already used.</response>
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(ApiResponse<LoginUserResponse>), 200)]
        [ProducesResponseType(typeof(ErrorDetails), 401)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken ct)
        {
            var result = await _mediator.Send(new RefreshTokenCommand(request.RefreshToken), ct);
            return Ok(ApiResponse<LoginUserResponse>.SuccessResponse(result, "Tokens refreshed successfully"));
        }
    }
}
