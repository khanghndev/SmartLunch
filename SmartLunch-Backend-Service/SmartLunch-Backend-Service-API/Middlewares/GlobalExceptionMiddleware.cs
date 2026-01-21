using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using rhetorai_service_api.Models.DTO;
using System.Net;
using System.Text.Json;

namespace rhetorai_service_api.Middleware
{
    /// <summary>
    /// Global exception handling middleware for consistent error responses
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred. RequestId: {RequestId}", 
                    httpContext.TraceIdentifier);
                
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var requestId = context.TraceIdentifier;

            var response = exception switch
            {
                ArgumentException ex => ApiResponse<object>.ErrorResult(
                    ex.Message, 
                    new List<string> { ex.Message }, 
                    (int)HttpStatusCode.BadRequest),
                
                UnauthorizedAccessException ex => ApiResponse<object>.ErrorResult(
                    "Unauthorized access", 
                    new List<string> { ex.Message }, 
                    (int)HttpStatusCode.Unauthorized),
                
                KeyNotFoundException ex => ApiResponse<object>.NotFoundResult(ex.Message),
                
                InvalidOperationException ex => ApiResponse<object>.ErrorResult(
                    ex.Message, 
                    new List<string> { ex.Message }, 
                    (int)HttpStatusCode.BadRequest),
                
                TimeoutException ex => ApiResponse<object>.ErrorResult(
                    "Request timeout", 
                    new List<string> { ex.Message }, 
                    (int)HttpStatusCode.RequestTimeout),
                
                _ => ApiResponse<object>.ErrorResult(
                    "An internal server error occurred", 
                    new List<string> { "Please contact support if the problem persists" }, 
                    (int)HttpStatusCode.InternalServerError)
            };

            response.RequestId = requestId;
            response.Timestamp = DateTime.UtcNow;

            context.Response.StatusCode = response.StatusCode;

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }

    /// <summary>
    /// Extension method to register the global exception middleware
    /// </summary>
    public static class GlobalExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionMiddleware>();
        }
    }
}