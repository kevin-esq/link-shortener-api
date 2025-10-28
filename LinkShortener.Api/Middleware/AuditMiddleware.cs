using LinkShortener.Application.Abstractions;
using System.Security.Claims;

namespace LinkShortener.Api.Middleware
{
    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;

        public AuditMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAuditService auditService)
        {
            var requestId = Guid.NewGuid().ToString();
            context.Items["RequestId"] = requestId;

            var shouldAudit = ShouldAuditRequest(context);

            if (shouldAudit)
            {
                await AuditRequestAsync(context, auditService, requestId);
            }

            await _next(context);
        }

        private bool ShouldAuditRequest(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower() ?? "";
            var method = context.Request.Method;

            if (path.Contains("/health") || path.Contains("/metrics") || path.Contains("/swagger"))
                return false;

            if (path.StartsWith("/s/"))
                return false;

            if (method == "GET" && !path.Contains("/admin"))
                return false;

            if (method == "POST" || method == "PUT" || method == "DELETE" || method == "PATCH")
                return true;

            if (path.Contains("/admin") || path.Contains("/api/users") || path.Contains("/api/auth"))
                return true;

            return false;
        }

        private async Task AuditRequestAsync(HttpContext context, IAuditService auditService, string requestId)
        {
            try
            {
                var user = context.User;
                var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var username = user?.FindFirst(ClaimTypes.Email)?.Value ?? "anonymous";
                var role = user?.FindFirst(ClaimTypes.Role)?.Value ?? "anonymous";

                var action = $"{context.Request.Method.ToLower()}.{context.Request.Path.Value?.TrimStart('/').Replace("/", ".")}";
                var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var userAgent = context.Request.Headers["User-Agent"].ToString();

                await auditService.LogAsync(
                    userId != null ? Guid.Parse(userId) : null,
                    username,
                    role,
                    ipAddress,
                    userAgent,
                    action,
                    "http_request",
                    requestId: requestId,
                    metadata: new Dictionary<string, object>
                    {
                        ["method"] = context.Request.Method,
                        ["path"] = context.Request.Path.Value ?? "",
                        ["query"] = context.Request.QueryString.Value ?? ""
                    }
                );
            }
            catch
            {
                // Silent fail
            }
        }
    }
}
