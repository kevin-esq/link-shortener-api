using LinkShortener.Api.Attributes;
using LinkShortener.Application.Common.Models;
using LinkShortener.Application.Features.Admin.Commands;
using LinkShortener.Application.Features.Admin.DTOs;
using LinkShortener.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkShortener.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [RequireRole(Role.Admin)]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all users with pagination
        /// </summary>
        [HttpGet("users")]
        [ProducesResponseType(typeof(ApiResponse<List<UserListResponse>>), 200)]
        public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken ct = default)
        {
            var users = await _mediator.Send(new GetAllUsersQuery(page, pageSize), ct);
            return Ok(ApiResponse<List<UserListResponse>>.SuccessResponse(users));
        }

        /// <summary>
        /// Get detailed user information
        /// </summary>
        [HttpGet("users/{userId}")]
        [ProducesResponseType(typeof(ApiResponse<UserDetailResponse>), 200)]
        [ProducesResponseType(typeof(ErrorDetails), 404)]
        public async Task<IActionResult> GetUserDetail(Guid userId, CancellationToken ct)
        {
            var user = await _mediator.Send(new GetUserDetailQuery(userId), ct);
            return Ok(ApiResponse<UserDetailResponse>.SuccessResponse(user));
        }

        /// <summary>
        /// Suspend a user account
        /// </summary>
        [HttpPost("users/{userId}/suspend")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ErrorDetails), 404)]
        public async Task<IActionResult> SuspendUser(Guid userId, [FromBody] SuspendUserRequest request, CancellationToken ct)
        {
            await _mediator.Send(new SuspendUserCommand(userId, request.Reason), ct);
            return Ok(ApiResponse.SuccessResponse("User suspended successfully"));
        }

        /// <summary>
        /// Ban a user account permanently
        /// </summary>
        [HttpPost("users/{userId}/ban")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ErrorDetails), 404)]
        public async Task<IActionResult> BanUser(Guid userId, [FromBody] BanUserRequest request, CancellationToken ct)
        {
            await _mediator.Send(new BanUserCommand(userId, request.Reason), ct);
            return Ok(ApiResponse.SuccessResponse("User banned successfully"));
        }

        /// <summary>
        /// Unsuspend a user account
        /// </summary>
        [HttpPost("users/{userId}/unsuspend")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ErrorDetails), 404)]
        public async Task<IActionResult> UnsuspendUser(Guid userId, CancellationToken ct)
        {
            await _mediator.Send(new UnsuspendUserCommand(userId), ct);
            return Ok(ApiResponse.SuccessResponse("User unsuspended successfully"));
        }

        /// <summary>
        /// Add a role to a user
        /// </summary>
        [HttpPost("users/{userId}/roles/{role}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ErrorDetails), 404)]
        public async Task<IActionResult> AddRole(Guid userId, Role role, CancellationToken ct)
        {
            await _mediator.Send(new AddRoleCommand(userId, role), ct);
            return Ok(ApiResponse.SuccessResponse($"Role {role} added successfully"));
        }

        /// <summary>
        /// Remove a role from a user
        /// </summary>
        [HttpDelete("users/{userId}/roles/{role}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ErrorDetails), 404)]
        public async Task<IActionResult> RemoveRole(Guid userId, Role role, CancellationToken ct)
        {
            await _mediator.Send(new RemoveRoleCommand(userId, role), ct);
            return Ok(ApiResponse.SuccessResponse($"Role {role} removed successfully"));
        }

        /// <summary>
        /// Get all sessions for a user
        /// </summary>
        [HttpGet("users/{userId}/sessions")]
        [ProducesResponseType(typeof(ApiResponse<List<SessionResponse>>), 200)]
        public async Task<IActionResult> GetUserSessions(Guid userId, CancellationToken ct)
        {
            var sessions = await _mediator.Send(new GetUserSessionsQuery(userId), ct);
            return Ok(ApiResponse<List<SessionResponse>>.SuccessResponse(sessions));
        }

        /// <summary>
        /// End a specific user session
        /// </summary>
        [HttpDelete("users/{userId}/sessions/{sessionId}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ErrorDetails), 404)]
        public async Task<IActionResult> EndSession(Guid userId, Guid sessionId, CancellationToken ct)
        {
            await _mediator.Send(new EndUserSessionCommand(userId, sessionId), ct);
            return Ok(ApiResponse.SuccessResponse("Session ended successfully"));
        }

        /// <summary>
        /// End all sessions for a user
        /// </summary>
        [HttpDelete("users/{userId}/sessions")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        public async Task<IActionResult> EndAllSessions(Guid userId, CancellationToken ct)
        {
            await _mediator.Send(new EndAllUserSessionsCommand(userId), ct);
            return Ok(ApiResponse.SuccessResponse("All sessions ended successfully"));
        }

        /// <summary>
        /// Revoke all refresh tokens for a user
        /// </summary>
        [HttpPost("users/{userId}/revoke-tokens")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        public async Task<IActionResult> RevokeAllTokens(Guid userId, CancellationToken ct)
        {
            await _mediator.Send(new RevokeAllRefreshTokensCommand(userId), ct);
            return Ok(ApiResponse.SuccessResponse("All refresh tokens revoked successfully"));
        }
    }
}
