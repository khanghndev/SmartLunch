using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Khoa_Luan_KS_Web.Controllers
{
    [Authorize(Policy = "CustomerArea")]
    public class ProfileController : Controller
    {
        private readonly Services.BackendAuthClient _backendAuthClient;

        public ProfileController(Services.BackendAuthClient backendAuthClient)
        {
            _backendAuthClient = backendAuthClient;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var accessToken = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(accessToken))
                return RedirectToAction("Login", "Auth");

            try
            {
                var profile = await _backendAuthClient.GetProfileAsync(accessToken, ct);
                return View(profile);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Không thể tải thông tin hồ sơ: " + ex.Message;
                return RedirectToAction("Index", "Customer");
            }
        }

        public IActionResult Orders() => View();
        public IActionResult Contracts() => View();
    }
}
