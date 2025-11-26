using System.Net;
using System.Text.Json;
using Loan_API_project.Exceptions;
using Loan_API_project.Models.DTO;

namespace Loan_API_project.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var statusCode = HttpStatusCode.InternalServerError;
            object? details = null;
            string message;

            switch (exception)
            {
                case NotFoundException ex:
                    statusCode = HttpStatusCode.NotFound;
                    message = ex.Message;
                    _logger.LogWarning(ex, ex.Message);
                    break;

                case UnauthorizedException ex:
                    statusCode = HttpStatusCode.Unauthorized;
                    message = ex.Message;
                    _logger.LogWarning(ex, ex.Message);
                    break;

                case ForbiddenException ex:
                    statusCode = HttpStatusCode.Forbidden;
                    message = ex.Message;
                    _logger.LogWarning(ex, ex.Message);
                    break;

                case BadRequestException ex:
                    statusCode = HttpStatusCode.BadRequest;
                    message = ex.Message;
                    _logger.LogWarning(ex, ex.Message);
                    break;

                case ValidationException ex:
                    statusCode = HttpStatusCode.BadRequest;
                    message = ex.Message;
                    details = ex.Errors;  // ✔ object instead of serialized string
                    _logger.LogWarning(ex, ex.Message);
                    break;

                default:
                    message = "დაფიქსირდა შიდა შეცდომა. გთხოვთ სცადოთ მოგვიანებით.";
                    _logger.LogError(exception, exception.Message);
                    break;
            }

            context.Response.StatusCode = (int)statusCode;

            var result = JsonSerializer.Serialize(new ErrorResponse
            {
                StatusCode = (int)statusCode,
                Message = message,
                Details = details?.ToString()
            }, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(result);
        }
    }
}
