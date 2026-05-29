using Khoa_Luan_KS_Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Khoa_Luan_KS_Web.Controllers
{
    public class ServiceController : Controller
    {
        private readonly BackendMasterDataClient _masterDataClient;

        public ServiceController(BackendMasterDataClient masterDataClient)
        {
            _masterDataClient = masterDataClient;
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
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    HttpContext.Session.Clear();
                    context.Result = Redirect(Request.Path + Request.QueryString);
                    return;
                }
            }
            await next();
        }

        public async Task<IActionResult> Factory(CancellationToken ct)
        {
            await LoadSegmentMenusAsync("industrial", ct);
            return View();
        }

        public async Task<IActionResult> Office(CancellationToken ct)
        {
            await LoadSegmentMenusAsync("org_company", ct);
            return View();
        }

        public async Task<IActionResult> School(CancellationToken ct)
        {
            await LoadSegmentMenusAsync("org_primary_school", ct);
            return View();
        }

        public IActionResult Safety() => View();
        public IActionResult MenuSuggestions() => View();

        private async Task LoadSegmentMenusAsync(string profileKey, CancellationToken ct)
        {
            ViewBag.SegmentProfileKey = profileKey;
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token))
            {
                ViewBag.SegmentMenuLoginRequired = true;
                return;
            }

            try
            {
                var res = await _masterDataClient.GetWeeklyMenusAsync(
                    token,
                    1,
                    8,
                    customerProfileKey: profileKey,
                    notEndedBefore: DateTime.Today,
                    ct: ct);
                ViewBag.SegmentMenus = WeeklyMenuCustomerVisibility.FilterOrderableWeeks(res.Items).ToList();
            }
            catch (Exception ex)
            {
                ViewBag.SegmentMenuError = ex.Message;
            }
        }
    }
}
