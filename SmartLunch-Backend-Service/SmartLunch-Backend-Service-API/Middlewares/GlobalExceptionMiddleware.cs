using System.Net;
using System.Text.Json;
using SmartLunch.Backend.Service.Application.DTOs;

namespace SmartLunch.Backend.Service.API.Middlewares
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

            var (baseResponse, statusCode) = exception switch
            {
                ArgumentException ex => (
                    BaseApiResponse<object>.ErrorResult(
                        ex.Message,
                        new List<string> { ex.Message }),
                    (int)HttpStatusCode.BadRequest),

                UnauthorizedAccessException ex => (
                    BaseApiResponse<object>.ErrorResult(
                        "Unauthorized access",
                        new List<string> { ex.Message }),
                    (int)HttpStatusCode.Unauthorized),

                KeyNotFoundException ex => (
                    BaseApiResponse<object>.NotFoundResult(ex.Message),
                    (int)HttpStatusCode.NotFound),

                InvalidOperationException ex => (
                    BaseApiResponse<object>.ErrorResult(
                        ex.Message,
                        new List<string> { ex.Message }),
                    (int)HttpStatusCode.BadRequest),

                TimeoutException ex => (
                    BaseApiResponse<object>.ErrorResult(
                        "Request timeout",
                        new List<string> { ex.Message }),
                    (int)HttpStatusCode.RequestTimeout),

                _ => (
                    BaseApiResponse<object>.ErrorResult(
                        "An internal server error occurred",
                        new List<string> { "Please contact support if the problem persists" }),
                    (int)HttpStatusCode.InternalServerError)
            };

            // Create a new response with RequestId included (since properties are init-only)
            var response = new BaseApiResponse<object>
            {
                Success = baseResponse.Success,
                ResponseTimestamp = baseResponse.ResponseTimestamp,
                ResponseType = baseResponse.ResponseType,
                ResponseSource = baseResponse.ResponseSource,
                ResponseMessage = baseResponse.ResponseMessage,
                ResponseStatus = baseResponse.ResponseStatus,
                ResponseData = default!,
                ResponseError = baseResponse.ResponseError,
                ResponseErrorDetails = baseResponse.ResponseErrorDetails,
                RequestId = requestId,
                Timestamp = baseResponse.Timestamp,
                Errors = baseResponse.Errors
            };

            context.Response.StatusCode = statusCode;

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