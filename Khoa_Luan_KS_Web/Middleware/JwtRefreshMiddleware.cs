using Khoa_Luan_KS_Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Khoa_Luan_KS_Web.Middleware;

/// <summary>
/// Tự làm mới JWT trước khi hết hạn cho mọi user đã đăng nhập (Admin, Manager, WarehouseStaff, Customer, Organization).
/// </summary>
public sealed class JwtRefreshMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<JwtRefreshMiddleware> _logger;

    public JwtRefreshMiddleware(RequestDelegate next, ILogger<JwtRefreshMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IApiTokenService tokenService)
    {
        var path = context.Request.Path.Value ?? "";

        if (IsAuthPage(path))
        {
            await _next(context);
            return;
        }

        var hasRefresh = !string.IsNullOrWhiteSpace(tokenService.GetRefreshToken());
        var isAuthenticated = context.User.Identity?.IsAuthenticated == true;

        if (hasRefresh && (isAuthenticated || !string.IsNullOrWhiteSpace(tokenService.GetAccessToken())))
        {
            var valid = await tokenService.EnsureValidAccessTokenAsync(context.RequestAborted);
            if (valid == null && isAuthenticated)
            {
                _logger.LogInformation("Session refresh failed; signing out user at {Path}", path);
                tokenService.ClearTokens();
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                var role = ResolveAreaRole(path);
                var returnUrl = context.Request.PathBase + context.Request.Path + context.Request.QueryString;
                context.Response.Redirect(
                    $"/Auth/Login?role={Uri.EscapeDataString(role)}&returnUrl={Uri.EscapeDataString(returnUrl)}");
                return;
            }
        }

        await _next(context);
    }

    private static bool IsAuthPage(string path) =>
        path.StartsWith("/Auth", StringComparison.OrdinalIgnoreCase);

    private static string ResolveAreaRole(string path)
    {
        if (path.StartsWith("/admin", StringComparison.OrdinalIgnoreCase)) return "Admin";
        if (path.StartsWith("/WarehouseStaff", StringComparison.OrdinalIgnoreCase)) return "WarehouseStaff";
        if (path.StartsWith("/manager", StringComparison.OrdinalIgnoreCase)) return "Manager";
        return "Customer";
    }
}
