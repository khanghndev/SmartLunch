using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Khoa_Luan_KS_Web.Areas.Manager.Models;

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

        public async Task<IActionResult> Index(
            int page = 1,
            int pageSize = 10,
            string? searchTerm = null,
            string? roleName = null,
            CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            pageSize = pageSize switch
            {
                10 => 10,
                15 => 15,
                20 => 20,
                30 => 30,
                50 => 50,
                _ => 10
            };

            if (page < 1) page = 1;

            try
            {
                var response = await _adminClient.GetUsersAsync(
                    token, page, pageSize, searchTerm, roleName, ct: ct);

                var effectivePageSize = response.PageSize > 0 ? response.PageSize : pageSize;
                var totalPages = effectivePageSize > 0
                    ? Math.Max(1, (int)Math.Ceiling(response.TotalCount / (double)effectivePageSize))
                    : 1;
                var effectivePage = response.Page > 0 ? response.Page : page;

                if (response.TotalCount > 0 && effectivePage > totalPages)
                {
                    return RedirectToAction(nameof(Index), new
                    {
                        page = totalPages,
                        pageSize = effectivePageSize,
                        searchTerm,
                        roleName
                    });
                }

                ViewBag.CurrentPage = effectivePage;
                ViewBag.PageSize = effectivePageSize;
                ViewBag.TotalPages = totalPages;
                ViewBag.SearchTerm = searchTerm ?? "";
                ViewBag.RoleName = roleName ?? "";
                ViewBag.Pagination = new ManagerPaginationVm
                {
                    Area = "Admin",
                    Controller = "User",
                    Action = "Index",
                    Page = effectivePage,
                    PageSize = effectivePageSize,
                    TotalCount = response.TotalCount,
                    ItemCount = response.Items.Count,
                    TotalPages = totalPages,
                    SearchTerm = searchTerm,
                    RoleName = roleName
                };

                return View(response);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                ViewBag.CurrentPage = 1;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalPages = 1;
                ViewBag.SearchTerm = searchTerm ?? "";
                ViewBag.RoleName = roleName ?? "";
                ViewBag.Pagination = new ManagerPaginationVm
                {
                    Area = "Admin",
                    Controller = "User",
                    Action = "Index",
                    Page = 1,
                    PageSize = pageSize,
                    TotalCount = 0,
                    ItemCount = 0,
                    TotalPages = 1,
                    SearchTerm = searchTerm,
                    RoleName = roleName
                };
                return View(new Services.AdminGetUsersResponse());
            }
        }

        [HttpPost]
        public async Task<IActionResult> Lock(
            int id,
            int page = 1,
            int pageSize = 10,
            string? searchTerm = null,
            string? roleName = null,
            CancellationToken ct = default)
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
            return RedirectToIndex(page, pageSize, searchTerm, roleName);
        }

        [HttpPost]
        public async Task<IActionResult> Unlock(
            int id,
            int page = 1,
            int pageSize = 10,
            string? searchTerm = null,
            string? roleName = null,
            CancellationToken ct = default)
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
            return RedirectToIndex(page, pageSize, searchTerm, roleName);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(
            int id,
            string newPassword,
            int page = 1,
            int pageSize = 10,
            string? searchTerm = null,
            string? roleName = null,
            CancellationToken ct = default)
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
            return RedirectToIndex(page, pageSize, searchTerm, roleName);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromForm] Services.CreateUserRequest request,
            int page = 1,
            int pageSize = 10,
            string? searchTerm = null,
            string? roleName = null,
            CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            try
            {
                await _adminClient.CreateUserAsync(request, token!, ct);
                TempData["Success"] = "Đã tạo tài khoản thành công.";
                return RedirectToIndex(1, pageSize, searchTerm, roleName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToIndex(page, pageSize, searchTerm, roleName);
        }

        [HttpPost]
        public async Task<IActionResult> Update(
            int id,
            [FromForm] Services.UpdateUserRequest request,
            string? role,
            int page = 1,
            int pageSize = 10,
            string? searchTerm = null,
            string? roleName = null,
            CancellationToken ct = default)
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
            return RedirectToIndex(page, pageSize, searchTerm, roleName);
        }

        private IActionResult RedirectToIndex(
            int page,
            int pageSize,
            string? searchTerm,
            string? roleName) =>
            RedirectToAction(nameof(Index), new { page, pageSize, searchTerm, roleName });
    }
}
