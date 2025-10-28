using LinkShortener.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkShortener.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AuditController : ControllerBase
    {
        private readonly IAuditService _auditService;

        public AuditController(IAuditService auditService)
        {
            _auditService = auditService;
        }

        [HttpGet("logs")]
        public async Task<IActionResult> GetLogs(
            [FromQuery] Guid? actorId,
            [FromQuery] string? action,
            [FromQuery] string? targetType,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            CancellationToken cancellationToken = default)
        {
            var logs = await _auditService.QueryAsync(
                actorId,
                action,
                targetType,
                fromDate,
                toDate,
                page,
                pageSize,
                cancellationToken
            );

            return Ok(new
            {
                success = true,
                data = logs,
                pagination = new
                {
                    page,
                    pageSize,
                    hasMore = logs.Count == pageSize
                }
            });
        }

        [HttpGet("actions")]
        public IActionResult GetAvailableActions()
        {
            var actions = typeof(Domain.Entities.AuditActions)
                .GetFields()
                .Select(f => new { name = f.Name, value = f.GetValue(null)?.ToString() })
                .ToList();

            return Ok(new { success = true, data = actions });
        }
    }
}
