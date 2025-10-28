using LinkShortener.Application.Features.Analytics.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LinkShortener.Api.Controllers
{
    /// <summary>
    /// Analytics API endpoints for LinkPulse analytics system.
    /// Provides detailed metrics and dashboard data for link tracking.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AnalyticsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AnalyticsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get comprehensive analytics for a specific link.
        /// </summary>
        /// <param name="linkId">The ID of the link to analyze</param>
        /// <param name="days">Number of days to analyze (default: 30)</param>
        /// <returns>Detailed analytics including clicks by date, country, device, browser, and referers</returns>
        /// <response code="200">Returns analytics data for the specified link</response>
        /// <response code="404">Link not found</response>
        /// <response code="401">User not authenticated</response>
        [HttpGet("link/{linkId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetLinkAnalytics(
            [FromRoute] Guid linkId,
            [FromQuery] int days = 30)
        {
            if (days < 1 || days > 365)
                return BadRequest("Days parameter must be between 1 and 365");

            var result = await _mediator.Send(new GetLinkAnalyticsQuery(linkId, days));

            if (result == null)
                return NotFound("Link not found");

            return Ok(result);
        }

        /// <summary>
        /// Get dashboard analytics for the authenticated user.
        /// Shows aggregated metrics across all user's links.
        /// </summary>
        /// <param name="days">Number of days to analyze (default: 30)</param>
        /// <returns>Dashboard with total clicks, top links, trends, and breakdowns</returns>
        /// <response code="200">Returns dashboard analytics for the user</response>
        /// <response code="401">User not authenticated</response>
        [HttpGet("dashboard")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUserDashboard([FromQuery] int days = 30)
        {
            if (days < 1 || days > 365)
                return BadRequest("Days parameter must be between 1 and 365");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized("User ID not found in token");

            var result = await _mediator.Send(new GetUserDashboardQuery(userId, days));

            return Ok(result);
        }

        /// <summary>
        /// Get quick stats summary for the authenticated user.
        /// Lightweight endpoint for displaying key metrics.
        /// </summary>
        /// <returns>Quick summary of user's link performance</returns>
        /// <response code="200">Returns summary statistics</response>
        /// <response code="401">User not authenticated</response>
        [HttpGet("summary")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetSummary()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized("User ID not found in token");

            // Get lightweight dashboard data for last 7 days
            var result = await _mediator.Send(new GetUserDashboardQuery(userId, 7));

            var summary = new
            {
                result.TotalLinks,
                result.TotalClicks,
                result.ClicksLast24Hours,
                result.ClicksLast7Days,
                TopLink = result.TopLinks.FirstOrDefault()
            };

            return Ok(summary);
        }
    }
}
