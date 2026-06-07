using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Khoa_Luan_KS_Web.Helpers;

namespace Khoa_Luan_KS_Web.Services;

public sealed class ApiTokenService : IApiTokenService
{
    private static readonly SemaphoreSlim RefreshLock = new(1, 1);

    public const string AccessTokenSessionKey = "access_token";
    public const string RefreshTokenSessionKey = "refresh_token";
    public const string AccessTokenCookieKey = "hm_access_token";
    public const string RefreshTokenCookieKey = "hm_refresh_token";
    public const string UserEmailCookieKey = "hm_user_email";
    public const string UserNameCookieKey = "hm_user_name";

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly BackendAuthClient _backendAuthClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ApiTokenService> _logger;

    public ApiTokenService(
        IHttpContextAccessor httpContextAccessor,
        BackendAuthClient backendAuthClient,
        IConfiguration configuration,
        ILogger<ApiTokenService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _backendAuthClient = backendAuthClient;
        _configuration = configuration;
        _logger = logger;
    }

    private HttpContext? Context => _httpContextAccessor.HttpContext;

    public string? GetAccessToken() =>
        Context?.Session.GetString(AccessTokenSessionKey)
        ?? Context?.Request.Cookies[AccessTokenCookieKey];

    public string? GetRefreshToken() =>
        Context?.Session.GetString(RefreshTokenSessionKey)
        ?? Context?.Request.Cookies[RefreshTokenCookieKey];

    public void PersistTokens(string accessToken, string refreshToken, string? email = null, string? displayName = null)
    {
        var ctx = Context;
        if (ctx == null)
            return;

        ctx.Session.SetString(AccessTokenSessionKey, accessToken);
        ctx.Session.SetString(RefreshTokenSessionKey, refreshToken);
        if (!string.IsNullOrWhiteSpace(email))
            ctx.Session.SetString("user_email", email);
        if (!string.IsNullOrWhiteSpace(displayName))
            ctx.Session.SetString("user_name", displayName);

        var cookieDays = _configuration.GetValue("Auth:TokenCookieExpireDays", 7);
        var opts = new CookieOptions
        {
            HttpOnly = true,
            Secure = ctx.Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(cookieDays),
        };

        ctx.Response.Cookies.Append(AccessTokenCookieKey, accessToken, opts);
        ctx.Response.Cookies.Append(RefreshTokenCookieKey, refreshToken, opts);
        if (!string.IsNullOrWhiteSpace(email))
            ctx.Response.Cookies.Append(UserEmailCookieKey, email, opts);
        if (!string.IsNullOrWhiteSpace(displayName))
            ctx.Response.Cookies.Append(UserNameCookieKey, displayName, opts);
    }

    public void ClearTokens()
    {
        var ctx = Context;
        if (ctx == null)
            return;

        ctx.Session.Remove(AccessTokenSessionKey);
        ctx.Session.Remove(RefreshTokenSessionKey);
        ctx.Session.Remove("user_email");
        ctx.Session.Remove("user_name");

        foreach (var key in new[] { AccessTokenCookieKey, RefreshTokenCookieKey, UserEmailCookieKey, UserNameCookieKey })
            ctx.Response.Cookies.Delete(key);
    }

    public async Task<string?> EnsureValidAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        var access = GetAccessToken();
        var refresh = GetRefreshToken();

        if (string.IsNullOrWhiteSpace(refresh))
            return string.IsNullOrWhiteSpace(access) ? null : access;

        if (!string.IsNullOrWhiteSpace(access) && !IsAccessTokenExpiringSoon(access))
            return access;

        return await RefreshInternalAsync(refresh, cancellationToken);
    }

    public async Task<string?> ForceRefreshAsync(CancellationToken cancellationToken = default)
    {
        var refresh = GetRefreshToken();
        if (string.IsNullOrWhiteSpace(refresh))
            return GetAccessToken();

        return await RefreshInternalAsync(refresh, cancellationToken);
    }

    public bool IsAccessTokenExpiringSoon(string? accessToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
            return true;

        try
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
            var exp = jwt.ValidTo;
            if (exp == DateTime.MinValue)
                return true;

            var skewMinutes = _configuration.GetValue("Auth:AccessTokenRefreshSkewMinutes", 5);
            return exp.ToUniversalTime() <= DateTime.UtcNow.AddMinutes(skewMinutes);
        }
        catch
        {
            return true;
        }
    }

    private async Task<string?> RefreshInternalAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var ctx = Context;
        if (ctx == null)
            return null;

        await RefreshLock.WaitAsync(cancellationToken);
        try
        {
            // Sau khi chờ lock, token có thể đã được request khác làm mới.
            var currentAccess = GetAccessToken();
            var currentRefresh = GetRefreshToken();
            if (!string.IsNullOrWhiteSpace(currentAccess) && !IsAccessTokenExpiringSoon(currentAccess))
                return currentAccess;

            if (!string.Equals(currentRefresh, refreshToken, StringComparison.Ordinal) &&
                !string.IsNullOrWhiteSpace(currentRefresh))
            {
                refreshToken = currentRefresh;
            }

            var result = await _backendAuthClient.RefreshTokenAsync(refreshToken, cancellationToken);
            if (string.IsNullOrWhiteSpace(result.AccessToken) || string.IsNullOrWhiteSpace(result.RefreshToken))
                return null;

            var email = ctx.Session.GetString("user_email") ?? ctx.Request.Cookies[UserEmailCookieKey];
            var name = ctx.Session.GetString("user_name") ?? ctx.Request.Cookies[UserNameCookieKey];
            PersistTokens(result.AccessToken, result.RefreshToken, email, name);

            if (ctx.User.Identity?.IsAuthenticated == true)
            {
                var principal = BuildPrincipalFromJwt(result.AccessToken);
                await ctx.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(
                            _configuration.GetValue("Auth:CookieAuthExpireHours", 8)),
                    });
            }

            _logger.LogInformation("Access token refreshed for session (user still authenticated: {Auth})",
                ctx.User.Identity?.IsAuthenticated == true);

            return result.AccessToken;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Refresh token failed");
            return null;
        }
        finally
        {
            RefreshLock.Release();
        }
    }

    private static ClaimsPrincipal BuildPrincipalFromJwt(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);

        var identity = new ClaimsIdentity(
            token.Claims,
            CookieAuthenticationDefaults.AuthenticationScheme,
            ClaimTypes.Name,
            ClaimTypes.Role);

        ApplyDisplayNameClaim(identity, token.Claims);

        foreach (var roleName in token.Claims
                     .Where(c => c.Type == "role" || c.Type == ClaimTypes.Role)
                     .Select(c => c.Value)
                     .Distinct())
        {
            if (!identity.HasClaim(ClaimTypes.Role, roleName))
                identity.AddClaim(new Claim(ClaimTypes.Role, roleName));
        }

        return new ClaimsPrincipal(identity);
    }

    private static void ApplyDisplayNameClaim(ClaimsIdentity identity, IEnumerable<Claim> jwtClaims)
    {
        var claims = jwtClaims.ToList();
        var fullName = claims.FirstOrDefault(c => c.Type == "FullName")?.Value;
        var username = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value
                       ?? claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;
        var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var displayName = UserDisplayNameHelper.Resolve(fullName, email, username);

        var existing = identity.FindFirst(ClaimTypes.Name);
        if (existing != null)
            identity.RemoveClaim(existing);

        identity.AddClaim(new Claim(ClaimTypes.Name, displayName));

        if (!string.IsNullOrWhiteSpace(username) && !identity.HasClaim(c => c.Type == "Username"))
            identity.AddClaim(new Claim("Username", username));
    }
}
