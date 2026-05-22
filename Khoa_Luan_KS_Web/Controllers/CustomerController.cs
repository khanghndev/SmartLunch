using System.Diagnostics;
using Khoa_Luan_KS_Web.Models;
using Khoa_Luan_KS_Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Khoa_Luan_KS_Web.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly CustomerHomeFeaturedMenuService _featuredMenuService;
        private readonly BackendMasterDataClient _masterDataClient;

        public CustomerController(
            ILogger<CustomerController> logger,
            CustomerHomeFeaturedMenuService featuredMenuService,
            BackendMasterDataClient masterDataClient)
        {
            _logger = logger;
            _featuredMenuService = featuredMenuService;
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

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var token = HttpContext.Session.GetString("access_token");
            var model = await _featuredMenuService.LoadFeaturedDishesAsync(token, ct);
            return View(model);
        }
        public async Task<IActionResult> About(CancellationToken ct)
        {
            var model = new AboutPageViewModel();
            try
            {
                var data = await _masterDataClient.GetPublicCompanyDocumentsAsync(ct);
                model.Documents = data.Documents;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not load public company documents for About page");
            }
            return View(model);
        }
        public IActionResult Contact() => View();
        public IActionResult HuitMeal() => View();
        public IActionResult Achievements() => View();
        public IActionResult ChooseHuitMeal() => View();
        public async Task<IActionResult> Reviews(CancellationToken ct)
        {
            var model = new ReviewsPageViewModel();
            try
            {
                var pub = await _masterDataClient.GetPublicReviewsAsync(ct: ct);
                model.Reviews = pub.Reviews;
                model.AverageRating = pub.AverageRating;
                model.TotalCount = pub.TotalCount;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not load public reviews");
            }

            var token = HttpContext.Session.GetString("access_token");
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    model.Context = await _masterDataClient.GetReviewMeContextAsync(token, ct);
                    model.CanSubmitLoaded = true;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not load review context for user");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitReview([FromBody] CreateCustomerReviewClientRequest request, CancellationToken ct)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return Unauthorized(new { message = "Vui lòng đăng nhập tài khoản doanh nghiệp." });
            try
            {
                await _masterDataClient.CreateCustomerReviewAsync(request, token, ct);
                return Json(new { ok = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
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
