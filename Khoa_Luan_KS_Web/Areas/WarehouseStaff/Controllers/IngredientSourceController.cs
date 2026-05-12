using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khoa_Luan_KS_Web.Services;

namespace Khoa_Luan_KS_Web.Areas.WarehouseStaff.Controllers
{
    [Area("WarehouseStaff")]
    [Authorize(Policy = "WarehouseStaffArea")]
    public class IngredientSourceController : Controller
    {
        private readonly BackendWarehouseClient _client;
        private readonly BackendMasterDataClient _masterData;
        public IngredientSourceController(BackendWarehouseClient client, BackendMasterDataClient masterData)
        {
            _client = client;
            _masterData = masterData;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 20, string? searchTerm = null, int? partnerId = null, int? ingredientId = null, CancellationToken ct = default)
        {
            ViewData["Title"] = "Lô Hàng / Nguồn Gốc Nguyên Liệu";
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var response = await _client.GetIngredientSourcesAsync(token, page, pageSize, searchTerm, partnerId, ingredientId, ct);
                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.SearchTerm = searchTerm ?? "";
                ViewBag.PartnerId = partnerId;
                ViewBag.IngredientId = ingredientId;
                return View(response);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new GetIngredientSourcesClientResponse());
            }
        }

        public async Task<IActionResult> Create(int? ingredientId = null, CancellationToken ct = default)
        {
            ViewData["Title"] = "Ghi Nhận Lô Hàng Mới";
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var ingredients = await _client.GetIngredientsAsync(token, 1, 500, isActive: true, ct: ct);
                ViewBag.Ingredients = ingredients.Items;
            }
            catch { ViewBag.Ingredients = new List<IngredientClientDto>(); }

            try
            {
                var partners = await _masterData.GetPartnersAsync(token, 1, 200, null, ct);
                ViewBag.Partners = partners.Items;
            }
            catch { ViewBag.Partners = new List<PartnerDto>(); }

            ViewBag.SelectedIngredientId = ingredientId;
            return View(new CreateIngredientSourceClientRequest());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateIngredientSourceClientRequest request, CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                if (request.IngredientId <= 0)
                {
                    TempData["Error"] = "Vui lòng chọn nguyên liệu.";
                    return RedirectToAction(nameof(Create));
                }
                var result = await _client.CreateIngredientSourceAsync(request, token, ct);
                TempData["Success"] = $"Đã ghi nhận lô hàng mới: #{result.IngredientSource.BatchNumber ?? result.IngredientSource.Id.ToString()}";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Create), new { ingredientId = request.IngredientId });
            }
        }

        public async Task<IActionResult> Edit(int id, CancellationToken ct = default)
        {
            ViewData["Title"] = "Cập Nhật Lô Hàng";
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var response = await _client.GetIngredientSourceAsync(id, token, ct);
                try
                {
                    var ingredients = await _client.GetIngredientsAsync(token, 1, 500, isActive: true, ct: ct);
                    ViewBag.Ingredients = ingredients.Items;
                }
                catch { ViewBag.Ingredients = new List<IngredientClientDto>(); }
                try
                {
                    var partners = await _masterData.GetPartnersAsync(token, 1, 200, null, ct);
                    ViewBag.Partners = partners.Items;
                }
                catch { ViewBag.Partners = new List<PartnerDto>(); }
                ViewBag.SourceId = id;
                return View(response.IngredientSource);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateIngredientSourceClientRequest request, CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                await _client.UpdateIngredientSourceAsync(id, request, token, ct);
                TempData["Success"] = "Đã cập nhật thông tin lô hàng.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Edit), new { id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                await _client.DeleteIngredientSourceAsync(id, token, ct);
                TempData["Success"] = "Đã xóa lô hàng.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
