namespace LinkShortener.Application.Abstractions
{
    public interface IAuditService
    {
        Task LogAsync(
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
            CancellationToken cancellationToken = default);

        Task<List<AuditLogDto>> QueryAsync(
            Guid? actorId = null,
            string? action = null,
            string? targetType = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int page = 1,
            int pageSize = 50,
            CancellationToken cancellationToken = default);
    }

    public record AuditLogDto(
        Guid Id,
        DateTime Timestamp,
        Guid? ActorId,
        string ActorUsername,
        string ActorRole,
        string IpAddress,
        string Action,
        string TargetType,
        Guid? TargetId,
        string? TargetDisplay,
        string? ChangesBefore,
        string? ChangesAfter,
        string? Reason,
        string Outcome,
        string? Metadata);
}
