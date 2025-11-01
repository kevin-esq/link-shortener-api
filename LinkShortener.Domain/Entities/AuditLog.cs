using LinkShortener.Domain.Common;

namespace LinkShortener.Domain.Entities
{
    public class AuditLog : BaseEntity
    {
        private AuditLog() { }

        public static AuditLog Create(
            Guid? actorId,
            string actorUsername,
            string actorRole,
            string ipAddress,
            string userAgent,
            string action,
            string targetType,
            Guid? targetId,
            string? targetDisplay,
            string? changesBefore,
            string? changesAfter,
            string? reason,
            string? requestId,
            string? traceId,
            string outcome,
            string? metadata)
        {
            return new AuditLog
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow,
                ActorId = actorId,
                ActorUsername = actorUsername,
                ActorRole = actorRole,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                Action = action,
                TargetType = targetType,
                TargetId = targetId,
                TargetDisplay = targetDisplay,
                ChangesBefore = changesBefore,
                ChangesAfter = changesAfter,
                Reason = reason,
                RequestId = requestId,
                TraceId = traceId,
                Outcome = outcome,
                Metadata = metadata
            };
        }

        public DateTime Timestamp { get; private set; }
        public Guid? ActorId { get; private set; }
        public string ActorUsername { get; private set; } = string.Empty;
        public string ActorRole { get; private set; } = string.Empty;
        public string IpAddress { get; private set; } = string.Empty;
        public string UserAgent { get; private set; } = string.Empty;
        public string Action { get; private set; } = string.Empty;
        public string TargetType { get; private set; } = string.Empty;
        public Guid? TargetId { get; private set; }
        public string? TargetDisplay { get; private set; }
        public string? ChangesBefore { get; private set; }
        public string? ChangesAfter { get; private set; }
        public string? Reason { get; private set; }
        public string? RequestId { get; private set; }
        public string? TraceId { get; private set; }
        public string Outcome { get; private set; } = string.Empty;
        public string? Metadata { get; private set; }
    }

    public static class AuditActions
    {
        public const string UserCreate = "user.create";
        public const string UserUpdate = "user.update";
        public const string UserDelete = "user.delete";
        public const string UserLogin = "user.login";
        public const string UserLogout = "user.logout";
        public const string UserPasswordChange = "user.password_change";
        public const string UserEmailChange = "user.email_change";
        public const string UserRoleChange = "user.role_change";

        public const string LinkCreate = "link.create";
        public const string LinkUpdate = "link.update";
        public const string LinkDelete = "link.delete";
        public const string LinkExpire = "link.expire";

        public const string AdminElevation = "admin.elevation";
        public const string AdminApiKeyGenerate = "admin.api_key.generate";
        public const string AdminApiKeyRevoke = "admin.api_key.revoke";
        public const string AdminDataExport = "admin.data.export";
        public const string AdminConfigChange = "admin.config.change";

        public const string SecurityLogin2FA = "security.2fa.login";
        public const string SecurityLoginFailed = "security.login.failed";
        public const string SecurityLockout = "security.lockout";
        public const string SecurityUnauthorized = "security.unauthorized";
    }

    public static class AuditOutcome
    {
        public const string Success = "success";
        public const string Failure = "failure";
        public const string Blocked = "blocked";
        public const string Error = "error";
    }
}
