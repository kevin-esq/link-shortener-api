using LinkShortener.Application.Features.Url.Commands;
using LinkShortener.Application.Features.Url.DTOs;
using LinkShortener.Application.Features.Url.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LinkShortener.Api.Controllers
{
    /// <summary>
    /// Manages URL shortening, redirection, and retrieval of URL information.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UrlController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        /// <summary>
        /// Creates a shortened URL for the authenticated user.
        /// </summary>
        /// <remarks>
        /// This endpoint converts a long URL into a short code that can be easily shared.
        /// Requires authentication.
        ///
        /// **Example request:**
        /// ```json
        /// {
        ///   "url": "https://www.example.com/very/long/link"
        /// }
        /// ```
        /// </remarks>
        /// <param name="request">The long URL to shorten.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <response code="200">Returns the shortened URL information.</response>
        /// <response code="400">If the URL is missing or invalid.</response>
        /// <response code="401">If the user is not authorized.</response>
        [HttpPost("shorten")]
        [Authorize]
        public async Task<IActionResult> ShortenUrl([FromBody] ShortenUrlRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Url))
                return BadRequest("URL is required.");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("Invalid user token.");

            var result = await _mediator.Send(new ShortenUrlCommand(
                request.Url,
                Request.Scheme,
                Request.Host.ToString(),
                userId),
                cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Redirects to the original (long) URL using the provided short code.
        /// </summary>
        /// <remarks>
        /// This is a **public endpoint** — no authentication required.  
        /// When a user accesses a short link, they are redirected to the original destination.
        ///
        /// Each access is recorded (IP, user agent, and timestamp).
        /// </remarks>
        /// <param name="Code">The short code representing the long URL.</param>
        /// <returns>Redirects the user to the original URL.</returns>
        /// <response code="302">Redirects to the original URL.</response>
        /// <response code="404">If no URL is found for the given code.</response>
        [HttpGet("/s/{Code}")]
        [AllowAnonymous]
        public async Task<IActionResult> RedirectToLongUrl(string Code)
        {
            var result = await _mediator.Send(new GetPublicUrlInfoQuery(
                Code,
                Request.Scheme,
                Request.Host.ToString()),
                CancellationToken.None);

            if (result == null)
                return NotFound();

            // Retrieve client IP
            var ipAddress = Request.HttpContext.Connection.RemoteIpAddress;
            string ipString = ipAddress == null
                ? "unknown"
                : (ipAddress.IsIPv4MappedToIPv6 ? ipAddress.MapToIPv4().ToString() : ipAddress.ToString());

            // Record link access
            await _mediator.Send(new RegisterLinkAccessCommand(
                LinkId: result.Id,
                UserId: null,
                IpAddress: ipString,
                UserAgent: Request.Headers.UserAgent.ToString()
            ));

            return Redirect(result.OriginalUrl);
        }

        /// <summary>
        /// Retrieves detailed information about a shortened URL (requires authentication).
        /// </summary>
        /// <remarks>
        /// Returns full information about a shortened URL, including creation date, original link, and usage stats.
        /// Only accessible to the owner of the URL.
        /// </remarks>
        /// <param name="code">The short code of the URL.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <response code="200">Returns detailed information about the shortened URL.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="404">If the URL was not found or does not belong to the user.</response>
        [Authorize]
        [HttpGet("info/{code}")]
        public async Task<IActionResult> GetInfo(string code, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim is null)
                return Unauthorized();

            var userId = Guid.Parse(userIdClaim);

            var result = await _mediator.Send(new GetPrivateUrlInfoQuery(
                code,
                Request.Scheme,
                Request.Host.ToString(),
                userId),
                cancellationToken);

            if (result is null)
                return NotFound();

            return Ok(result);
        }
    }
}
