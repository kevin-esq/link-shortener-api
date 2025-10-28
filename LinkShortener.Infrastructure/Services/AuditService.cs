using LinkShortener.Application.Abstractions;
using LinkShortener.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace LinkShortener.Infrastructure.Services
{
    public class AuditService : IAuditService
    {
        private readonly ApplicationDbContext _context;

        public AuditService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(
            Guid? actorId,
            string actorUsername,
            string actorRole,
            string ipAddress,
            string userAgent,
            string action,
            string targetType,
            Guid? targetId = null,
            string? targetDisplay = null,
            object? changesBefore = null,
            object? changesAfter = null,
            string? reason = null,
            string? requestId = null,
            string? traceId = null,
            string outcome = "success",
            Dictionary<string, object>? metadata = null,
            CancellationToken cancellationToken = default)
        {
            var auditLog = AuditLog.Create(
                actorId,
                actorUsername,
                actorRole,
                ipAddress,
                userAgent,
                action,
                targetType,
                targetId,
                targetDisplay,
                changesBefore != null ? JsonSerializer.Serialize(changesBefore) : null,
                changesAfter != null ? JsonSerializer.Serialize(changesAfter) : null,
                reason,
                requestId,
                traceId,
                outcome,
                metadata != null ? JsonSerializer.Serialize(metadata) : null
            );

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<AuditLogDto>> QueryAsync(
            Guid? actorId = null,
            string? action = null,
            string? targetType = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int page = 1,
            int pageSize = 50,
            CancellationToken cancellationToken = default)
        {
            var query = _context.AuditLogs.AsQueryable();

            if (actorId.HasValue)
                query = query.Where(a => a.ActorId == actorId.Value);

            if (!string.IsNullOrEmpty(action))
                query = query.Where(a => a.Action == action);

            if (!string.IsNullOrEmpty(targetType))
                query = query.Where(a => a.TargetType == targetType);

            if (fromDate.HasValue)
                query = query.Where(a => a.Timestamp >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(a => a.Timestamp <= toDate.Value);

            var logs = await query
                .OrderByDescending(a => a.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AuditLogDto(
                    a.Id,
                    a.Timestamp,
                    a.ActorId,
                    a.ActorUsername,
                    a.ActorRole,
                    a.IpAddress,
                    a.Action,
                    a.TargetType,
                    a.TargetId,
                    a.TargetDisplay,
                    a.ChangesBefore,
                    a.ChangesAfter,
                    a.Reason,
                    a.Outcome,
                    a.Metadata
                ))
                .ToListAsync(cancellationToken);

            return logs;
        }
    }
}
