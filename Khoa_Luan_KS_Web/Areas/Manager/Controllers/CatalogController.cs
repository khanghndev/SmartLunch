using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Khoa_Luan_KS_Web.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Policy = "ManagerArea")]
    public class CatalogController : Controller
    {
        private readonly Services.BackendMasterDataClient _masterDataClient;

        public CatalogController(Services.BackendMasterDataClient masterDataClient)
        {
            _masterDataClient = masterDataClient;
        }

        public async Task<IActionResult> Employees(int page = 1, int pageSize = 10, string? searchTerm = null, CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var response = await _masterDataClient.GetUsersAsync(token, page, pageSize, searchTerm, ct);
                return View(response);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new Services.AdminGetUsersResponse());
            }
        }

        [HttpPost]
        public async Task<IActionResult> LockEmployee(int id, CancellationToken ct)
        {
            var token = HttpContext.Session.GetString("access_token");
            try
            {
                await _masterDataClient.LockUserAsync(id, token!, ct);
                TempData["Success"] = "Đã khóa hồ sơ nhân viên thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Employees));
        }

        [HttpPost]
        public async Task<IActionResult> UnlockEmployee(int id, CancellationToken ct)
        {
            var token = HttpContext.Session.GetString("access_token");
            try
            {
                await _masterDataClient.UnlockUserAsync(id, token!, ct);
                TempData["Success"] = "Đã khôi phục hồ sơ nhân viên thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Employees));
        }

        [HttpPost]
        public async Task<IActionResult> ResetEmployeePassword(int id, string newPassword, CancellationToken ct)
        {
            var token = HttpContext.Session.GetString("access_token");
            try
            {
                await _masterDataClient.ResetPasswordAsync(id, newPassword, token!, ct);
                TempData["Success"] = "Đã đặt lại mật khẩu nhân viên thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Employees));
        }

        public IActionResult EmployeeDetail(string id) => View();

        public async Task<IActionResult> Suppliers(int page = 1, int pageSize = 10, string? searchTerm = null, CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var response = await _masterDataClient.GetPartnersAsync(token, page, pageSize, searchTerm, ct);
                return View(response);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new Services.GetPartnersResponse());
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSupplier(Services.CreatePartnerRequest request, CancellationToken ct)
        {
            var token = HttpContext.Session.GetString("access_token");
            try
            {
                await _masterDataClient.CreatePartnerAsync(request, token!, ct);
                TempData["Success"] = "Đã thêm nhà cung cấp mới thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Suppliers));
        }

        public async Task<IActionResult> SupplierDetail(int id, CancellationToken ct)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var response = await _masterDataClient.GetPartnerAsync(id, token, ct);
                return View(response.Partner);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Suppliers));
            }
        }
        public async Task<IActionResult> Customers(int page = 1, int pageSize = 10, string? searchTerm = null, CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var response = await _masterDataClient.GetUnitsAsync(token, page, pageSize, searchTerm, ct);
                return View(response);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new Services.GetUnitsResponse());
            }
        }
        public IActionResult CustomerDetail(string id) => View();

        public async Task<IActionResult> Meals(int page = 1, int pageSize = 20, string? searchTerm = null, string? category = null, CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var response = await _masterDataClient.GetDishesAsync(token, page, pageSize, searchTerm, category, ct);
                return View(response);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new Services.GetDishesResponse());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMeal([FromBody] Services.CreateDishRequest request, CancellationToken ct)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return Unauthorized(new { message = "Not authenticated" });

            try
            {
                var dish = await _masterDataClient.CreateDishAsync(request, token!, ct);
                TempData["Success"] = $"Đã thêm món \"{dish.Name}\" vào thực đơn thành công.";
                return Json(new { success = true, redirectUrl = Url.Action(nameof(MealDetail), new { id = dish.Id }) });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        public async Task<IActionResult> MealDetail(int id, CancellationToken ct)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var response = await _masterDataClient.GetDishAsync(id, token, ct);
                return View(response);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Meals));
            }
        }

        public async Task<IActionResult> MealEdit(int id, CancellationToken ct)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var response = await _masterDataClient.GetDishAsync(id, token, ct);
                return View(response.Dish);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Meals));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadDishImage(IFormFile file, CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return Unauthorized(new { message = "Not authenticated" });

            try
            {
                var uploaded = await _masterDataClient.UploadMediaAsync(file, token!, mediaType: "image", purpose: "dish-image", isPublic: true, ct: ct);
                // Return mediaFileId only. The linking will happen during UpdateMeal.
                return Json(new { mediaFileId = uploaded.MediaFileId, objectName = uploaded.ObjectName, previewUrl = uploaded.Url });
            }
            catch (Exception ex)
            {
                Response.StatusCode = 400;
                return Json(new { message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateMeal(int id, [FromBody] Services.UpdateDishRequest request, CancellationToken ct)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return Unauthorized(new { message = "Not authenticated" });

            try
            {
                await _masterDataClient.UpdateDishAsync(id, request, token!, ct);
                TempData["Success"] = $"Đã cập nhật món \"{request.Name}\" thành công.";
                return Json(new { success = true, redirectUrl = Url.Action(nameof(MealDetail), new { id }) });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
