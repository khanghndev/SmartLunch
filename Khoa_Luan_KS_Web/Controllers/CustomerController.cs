using System.Diagnostics;
using Khoa_Luan_KS_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Khoa_Luan_KS_Web.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ILogger<CustomerController> logger)
        {
            _logger = logger;
        }

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
                    // If Admin/Manager tries to access customer area, sign them out so they can browse as guest
                    await HttpContext.SignOutAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
                    HttpContext.Session.Clear();

                    // Reload the same page as anonymous
                    context.Result = Redirect(Request.Path + Request.QueryString);
                    return;
                }
            }
            await next();
        }

        public IActionResult Index() => View();
        public IActionResult About() => View();
        public IActionResult Contact() => View();
        public IActionResult HuitMeal() => View();
        public IActionResult Achievements() => View();
        public IActionResult ChooseHuitMeal() => View();
        public IActionResult Reviews() => View();
        public IActionResult Partners() => View();
        public IActionResult Recruitment() => View();
        
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
