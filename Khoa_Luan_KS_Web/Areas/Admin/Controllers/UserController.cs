using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Khoa_Luan_KS_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminArea")]
    public class UserController : Controller
    {
        private readonly Services.BackendMasterDataClient _adminClient;

        public UserController(Services.BackendMasterDataClient adminClient)
        {
            _adminClient = adminClient;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? searchTerm = null, CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var response = await _adminClient.GetUsersAsync(token, page, pageSize, searchTerm, ct: ct);
                return View(response);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new Services.AdminGetUsersResponse());
            }
        }

        [HttpPost]
        public async Task<IActionResult> Lock(int id, CancellationToken ct)
        {
            var token = HttpContext.Session.GetString("access_token");
            try
            {
                await _adminClient.LockUserAsync(id, token!, ct);
                TempData["Success"] = "Đã khóa tài khoản thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Unlock(int id, CancellationToken ct)
        {
            var token = HttpContext.Session.GetString("access_token");
            try
            {
                await _adminClient.UnlockUserAsync(id, token!, ct);
                TempData["Success"] = "Đã mở khóa tài khoản thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(int id, string newPassword, CancellationToken ct)
        {
            var token = HttpContext.Session.GetString("access_token");
            try
            {
                await _adminClient.ResetPasswordAsync(id, newPassword, token!, ct);
                TempData["Success"] = "Đã đặt lại mật khẩu thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] Services.CreateUserRequest request, CancellationToken ct)
        {
            var token = HttpContext.Session.GetString("access_token");
            try
            {
                await _adminClient.CreateUserAsync(request, token!, ct);
                TempData["Success"] = "Đã tạo tài khoản thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, [FromForm] Services.UpdateUserRequest request, string? role, CancellationToken ct)
        {
            var token = HttpContext.Session.GetString("access_token");
            try
            {
                await _adminClient.UpdateUserAsync(id, request, token!, ct);
                TempData["Success"] = "Đã cập nhật tài khoản thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
