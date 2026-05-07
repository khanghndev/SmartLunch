using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Khoa_Luan_KS_Web.Controllers
{
    public class ServiceController : Controller
    {
        public override async Task OnActionExecutionAsync(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context, Microsoft.AspNetCore.Mvc.Filters.ActionExecutionDelegate next)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var roles = User.Claims
                    .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
                    .Select(c => c.Value);

                bool isCustomer = User.IsInRole("Customer") || 
                                 User.IsInRole("Organization") ||
                                 User.IsInRole("Khách hàng doanh nghiệp") || 
                                 User.IsInRole("Khách hàng cá nhân");

                if (!isCustomer)
                {
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    HttpContext.Session.Clear();
                    context.Result = Redirect(Request.Path + Request.QueryString);
                    return;
                }
            }
            await next();
        }
        public IActionResult Factory() => View();
        public IActionResult Office() => View();
        public IActionResult School() => View();
        public IActionResult Safety() => View();
        public IActionResult MenuSuggestions() => View();
    }
}
