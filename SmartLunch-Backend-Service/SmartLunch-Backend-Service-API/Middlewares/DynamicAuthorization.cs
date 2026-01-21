public class DynamicAuthorizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DynamicAuthorizationMiddleware> _logger;

    public DynamicAuthorizationMiddleware(RequestDelegate next, ILogger<DynamicAuthorizationMiddleware> logger)
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
            _logger.LogError(ex, "An error occurred while processing the request.");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("An error occurred while processing the request.");
        }
    }
}