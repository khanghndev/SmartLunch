using rhetorai_service_api.Data;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace rhetorai_service_api.Middlewares
{
    public class ValidationSession
    {
        private readonly RequestDelegate _next;

        public ValidationSession(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, RhetorAIServiceDBContext dbContext)
        {
            // Chỉ kiểm tra nếu người dùng đã được xác thực (có token)
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = context.User.FindFirst(JwtRegisteredClaimNames.Sub);
                var stampClaim = context.User.FindFirst("sessionStamp");

                if (userIdClaim != null && stampClaim != null)
                {
                    if (!long.TryParse(userIdClaim.Value, out var userId))
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Invalid user identifier in token.");
                        return;
                    }

                    var tokenStamp = stampClaim.Value;

                    var user = await dbContext.Users
                                              .AsNoTracking()
                                              .FirstOrDefaultAsync(u => u.Id == userId);

                    if (user == null ||
                        user.SessionStamp != tokenStamp)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Session has been invalidated.");
                        return;
                    }
                }
            }

            // Nếu mọi thứ ổn, chuyển request cho middleware tiếp theo
            await _next(context);
        }
    }
}