using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khoa_Luan_KS_Web.Services;

namespace Khoa_Luan_KS_Web.Areas.WarehouseStaff.Controllers
{
    [Area("WarehouseStaff")]
    [Authorize(Policy = "ManagerArea")]
    public class IngredientController : Controller
    {
        private readonly BackendWarehouseClient _client;
        public IngredientController(BackendWarehouseClient client) { _client = client; }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 20, string? searchTerm = null, bool? isActive = null, CancellationToken ct = default)
        {
            ViewData["Title"] = "Danh Mục Nguyên Liệu";
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var response = await _client.GetIngredientsAsync(token, page, pageSize, searchTerm, isActive, ct);
                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.SearchTerm = searchTerm ?? "";
                ViewBag.IsActiveFilter = isActive;
                return View(response);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new GetIngredientsClientResponse());
            }
        }

        public async Task<IActionResult> Detail(int id, CancellationToken ct = default)
        {
            ViewData["Title"] = "Chi Tiết Nguyên Liệu";
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var detail = await _client.GetIngredientAsync(id, token, ct);
                try
                {
                    var inv = await _client.GetIngredientInventoryDetailAsync(id, token, 10, ct);
                    ViewBag.Inventory = inv;
                }
                catch { /* may not exist for inactive ingredient */ }

                return View(detail.Ingredient);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
