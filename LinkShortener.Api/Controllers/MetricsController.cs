using LinkShortener.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkShortener.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MetricsController : ControllerBase
    {
        private readonly IClickEventService _clickEventService;

        public MetricsController(IClickEventService clickEventService)
        {
            _clickEventService = clickEventService;
        }

        [HttpGet("link/{linkId}/stats")]
        public async Task<IActionResult> GetLinkStats(
            Guid linkId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            CancellationToken cancellationToken)
        {
            var stats = await _clickEventService.GetStatsAsync(linkId, fromDate, toDate, cancellationToken);
            return Ok(new { success = true, data = stats });
        }

        [HttpGet("link/{linkId}/recent-clicks")]
        public async Task<IActionResult> GetRecentClicks(
            Guid linkId,
            CancellationToken cancellationToken,
            [FromQuery] int limit = 100)
        {
            var clicks = await _clickEventService.GetRecentClicksAsync(linkId, limit, cancellationToken);
            return Ok(new { success = true, data = clicks });
        }
    }
}
