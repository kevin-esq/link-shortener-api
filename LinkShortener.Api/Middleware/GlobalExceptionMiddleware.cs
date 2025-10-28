using LinkShortener.Application.Common.Models;
using System.Net;

namespace LinkShortener.Api.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger,
            IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var errorDetails = new ErrorDetails
            {
                Success = false,
                TraceId = context.TraceIdentifier
            };

            switch (exception)
            {
                case UnauthorizedAccessException:
                    errorDetails.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorDetails.Message = exception.Message;
                    break;

                case ArgumentException:
                    errorDetails.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorDetails.Message = exception.Message;
                    break;

                case InvalidOperationException:
                    errorDetails.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorDetails.Message = exception.Message;
                    break;

                case KeyNotFoundException:
                    errorDetails.StatusCode = (int)HttpStatusCode.NotFound;
                    errorDetails.Message = exception.Message;
                    break;

                default:
                    errorDetails.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorDetails.Message = _env.IsDevelopment()
                        ? exception.Message
                        : "An internal server error occurred";

                    if (_env.IsDevelopment() && exception.StackTrace != null)
                    {
                        errorDetails.Errors = new List<string> { exception.StackTrace };
                    }
                    break;
            }

            context.Response.StatusCode = errorDetails.StatusCode;
            await context.Response.WriteAsync(errorDetails.ToString());
        }
    }
}
