using LinkShortener.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace LinkShortener.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireRoleAttribute : Attribute, IAuthorizationFilter
    {
        private readonly UserRole[] _roles;

        public RequireRoleAttribute(params UserRole[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var userRoles = user.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            var hasRequiredRole = _roles.Any(role =>
                userRoles.Contains(role.ToString()));

            if (!hasRequiredRole)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
