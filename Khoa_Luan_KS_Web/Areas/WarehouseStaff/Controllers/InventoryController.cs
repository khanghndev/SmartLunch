using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khoa_Luan_KS_Web.Services;

namespace Khoa_Luan_KS_Web.Areas.WarehouseStaff.Controllers
{
    [Area("WarehouseStaff")]
    [Authorize(Policy = "WarehouseStaffArea")]
    public class InventoryController : Controller
    {
        private readonly BackendWarehouseClient _client;
        public InventoryController(BackendWarehouseClient client) { _client = client; }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 50, string? searchTerm = null, CancellationToken ct = default)
        {
            ViewData["Title"] = "Tồn Kho Hiện Tại";
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            var vm = new InventoryIndexViewModel
            {
                Page = page,
                PageSize = pageSize,
                SearchTerm = searchTerm ?? ""
            };

            try
            {
                var inv = await _client.GetInventoriesAsync(token, page, pageSize, searchTerm, ct);
                vm.Inventories = inv.Items;
                vm.TotalCount = inv.TotalCount;
            }
            catch (Exception ex) { TempData["Error"] = ex.Message; }

            try
            {
                var ing = await _client.GetIngredientsAsync(token, 1, 500, isActive: true, ct: ct);
                vm.IngredientMap = ing.Items.ToDictionary(i => i.Id, i => i);
            }
            catch { /* ignore */ }

            try
            {
                vm.LowStockCount = (await _client.GetLowStockAlertsAsync(token, ct)).Alerts.Count;
            }
            catch { /* ignore */ }

            return View(vm);
        }

        public async Task<IActionResult> LowStock(CancellationToken ct = default)
        {
            ViewData["Title"] = "Cảnh Báo Tồn Thấp";
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var alerts = await _client.GetLowStockAlertsAsync(token, ct);
                return View(alerts);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new GetLowStockAlertsClientResponse());
            }
        }

        public async Task<IActionResult> Detail(int id, CancellationToken ct = default)
        {
            ViewData["Title"] = "Chi Tiết Tồn Kho";
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var detail = await _client.GetIngredientInventoryDetailAsync(id, token, 30, ct);
                return View(detail);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }

    public class InventoryIndexViewModel
    {
        public List<InventoryClientDto> Inventories { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public string SearchTerm { get; set; } = "";
        public int LowStockCount { get; set; }
        public Dictionary<int, IngredientClientDto> IngredientMap { get; set; } = new();
    }
}
