using Microsoft.EntityFrameworkCore;
using rhetorai_service_api.Data;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace rhetorai_service_api.Middlewares
{
    /// <summary>
    /// Development-only middleware to bypass authentication for faster API development
    /// Usage: Add header "X-Dev-User" with username or "X-Dev-UserId" with user ID
    /// Or use "X-Dev-Token" with a valid JWT token
    /// </summary>
    public class DevelopmentAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<DevelopmentAuthMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public DevelopmentAuthMiddleware(
            RequestDelegate next,
            ILogger<DevelopmentAuthMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context, RhetorAIServiceDBContext dbContext)
        {
            // Only enable in Development environment
            if (!_environment.IsDevelopment())
            {
                await _next(context);
                return;
            }

            // Skip if already authenticated
            if (context.User.Identity?.IsAuthenticated == true)
            {
                await _next(context);
                return;
            }

            // Check for development authentication headers
            var devUserId = context.Request.Headers["X-Dev-UserId"].FirstOrDefault();
            var devUsername = context.Request.Headers["X-Dev-User"].FirstOrDefault();
            var devToken = context.Request.Headers["X-Dev-Token"].FirstOrDefault();

            if (!string.IsNullOrEmpty(devToken))
            {
                // Use provided token directly
                await SetUserFromToken(context, devToken);
            }
            else if (!string.IsNullOrEmpty(devUserId) && long.TryParse(devUserId, out var userId))
            {
                // Authenticate by user ID
                await SetUserFromId(context, dbContext, userId);
            }
            else if (!string.IsNullOrEmpty(devUsername))
            {
                // Authenticate by username
                await SetUserFromUsername(context, dbContext, devUsername);
            }

            await _next(context);
        }

        private async Task SetUserFromToken(HttpContext context, string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadJwtToken(token);

                var claims = jsonToken.Claims.ToList();
                var identity = new ClaimsIdentity(claims, "Development");
                context.User = new ClaimsPrincipal(identity);

                _logger.LogDebug("Development auth: Authenticated via token for user {UserId}",
                    claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Development auth: Failed to parse dev token");
            }
        }

        private async Task SetUserFromId(HttpContext context, RhetorAIServiceDBContext dbContext, long userId)
        {
            try
            {
                var user = await dbContext.Users
                    .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == userId && u.DeletedAt == null);

                if (user == null)
                {
                    _logger.LogWarning("Development auth: User with ID {UserId} not found", userId);
                    return;
                }

                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty, ClaimValueTypes.String),
                    new Claim("sessionStamp", user.SessionStamp ?? Guid.NewGuid().ToString(), ClaimValueTypes.String)
                };

                // Add role claims
                foreach (var userRole in user.UserRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, userRole.Role.RoleName));
                }

                var identity = new ClaimsIdentity(claims, "Development");
                context.User = new ClaimsPrincipal(identity);

                _logger.LogDebug("Development auth: Authenticated user {Username} (ID: {UserId}) with roles: {Roles}",
                    user.Username, userId, string.Join(", ", user.UserRoles.Select(ur => ur.Role.RoleName)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Development auth: Error authenticating user by ID {UserId}", userId);
            }
        }

        private async Task SetUserFromUsername(HttpContext context, RhetorAIServiceDBContext dbContext, string username)
        {
            try
            {
                var user = await dbContext.Users
                    .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Username == username && u.DeletedAt == null);

                if (user == null)
                {
                    _logger.LogWarning("Development auth: User with username {Username} not found", username);
                    return;
                }

                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty, ClaimValueTypes.String),
                    new Claim("sessionStamp", user.SessionStamp ?? Guid.NewGuid().ToString(), ClaimValueTypes.String)
                };

                // Add role claims
                foreach (var userRole in user.UserRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, userRole.Role.RoleName));
                }

                var identity = new ClaimsIdentity(claims, "Development");
                context.User = new ClaimsPrincipal(identity);

                _logger.LogDebug("Development auth: Authenticated user {Username} (ID: {UserId}) with roles: {Roles}",
                    user.Username, user.Id, string.Join(", ", user.UserRoles.Select(ur => ur.Role.RoleName)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Development auth: Error authenticating user by username {Username}", username);
            }
        }
    }

    /// <summary>
    /// Extension method to register the development auth middleware
    /// </summary>
    public static class DevelopmentAuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseDevelopmentAuth(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DevelopmentAuthMiddleware>();
        }
    }
}

