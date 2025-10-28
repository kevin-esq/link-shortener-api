using LinkShortener.Application.Abstractions;
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
    public class UrlController(IMediator mediator, IClickEventService clickEventService) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly IClickEventService _clickEventService = clickEventService;

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

            // Record link access with enhanced analytics
            var startTime = DateTime.UtcNow;
            
            await _mediator.Send(new RegisterLinkAccessCommand(
                LinkId: result.Id,
                IpAddress: ipString,
                UserAgent: Request.Headers.UserAgent.ToString() ?? "Unknown",
                UserId: null,
                Referer: Request.Headers.Referer.ToString()
            ));

            // Record click event for advanced metrics
            var utmParams = new Dictionary<string, string>();
            if (Request.Query.TryGetValue("utm_source", out var utmSource))
                utmParams["utm_source"] = utmSource.ToString();
            if (Request.Query.TryGetValue("utm_medium", out var utmMedium))
                utmParams["utm_medium"] = utmMedium.ToString();
            if (Request.Query.TryGetValue("utm_campaign", out var utmCampaign))
                utmParams["utm_campaign"] = utmCampaign.ToString();
            if (Request.Query.TryGetValue("utm_content", out var utmContent))
                utmParams["utm_content"] = utmContent.ToString();
            if (Request.Query.TryGetValue("utm_term", out var utmTerm))
                utmParams["utm_term"] = utmTerm.ToString();

            var latencyMs = (int)(DateTime.UtcNow - startTime).TotalMilliseconds;
            var requestId = Request.HttpContext.Items["RequestId"]?.ToString();

            await _clickEventService.RecordClickAsync(
                result.Id,
                Code,
                result.OriginalUrl,
                null,
                Request.Headers.Referer.ToString(),
                utmParams.Count > 0 ? utmParams : null,
                ipString,
                Request.Headers.UserAgent.ToString() ?? "Unknown",
                Request.Headers.AcceptLanguage.ToString(),
                Domain.Entities.ClickStatus.Redirected,
                latencyMs,
                "302",
                requestId
            );

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

        [Authorize]
        [HttpGet("my-links")]
        public async Task<IActionResult> GetMyLinks(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null,
            [FromQuery] string? orderBy = "createdAt",
            [FromQuery] string? orderDirection = "desc",
            CancellationToken cancellationToken = default)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim is null)
                return Unauthorized();

            var userId = Guid.Parse(userIdClaim);

            var result = await _mediator.Send(
                new GetUserLinksQuery(userId, page, pageSize, search, orderBy, orderDirection), 
                cancellationToken);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("{code}")]
        public async Task<IActionResult> DeleteLink(string code, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim is null)
                return Unauthorized();

            var userId = Guid.Parse(userIdClaim);

            var result = await _mediator.Send(new DeleteLinkCommand(code, userId), cancellationToken);
            
            if (!result)
                return NotFound();

            return NoContent();
        }

        [Authorize]
        [HttpGet("{code}/stats")]
        public async Task<IActionResult> GetLinkStats(
            string code,
            [FromQuery] int days = 30,
            CancellationToken cancellationToken = default)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim is null)
                return Unauthorized();

            var userId = Guid.Parse(userIdClaim);

            var result = await _mediator.Send(new GetLinkStatsQuery(code, userId, days), cancellationToken);
            
            if (result is null)
                return NotFound();

            return Ok(result);
        }

        [Authorize]
        [HttpGet("{code}/qr")]
        public async Task<IActionResult> GetQrCode(
            string code,
            [FromQuery] int size = 300,
            CancellationToken cancellationToken = default)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim is null)
                return Unauthorized();

            var userId = Guid.Parse(userIdClaim);

            var result = await _mediator.Send(new GetQrCodeQuery(code, userId, size), cancellationToken);
            
            if (result is null)
                return NotFound();

            return File(result.QrCodeImage, "image/png", $"{code}_qr.png");
        }
    }
}
