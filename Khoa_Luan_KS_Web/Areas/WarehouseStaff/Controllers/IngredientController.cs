using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khoa_Luan_KS_Web.Services;

namespace Khoa_Luan_KS_Web.Areas.WarehouseStaff.Controllers
{
    [Area("WarehouseStaff")]
    [Authorize(Policy = "WarehouseStaffArea")]
    public class IngredientController : Controller
    {
        private readonly BackendWarehouseClient _client;
        private readonly BackendMasterDataClient _masterData;

        public IngredientController(BackendWarehouseClient client, BackendMasterDataClient masterData)
        {
            _client = client;
            _masterData = masterData;
        }

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

        public async Task<IActionResult> Create(CancellationToken ct = default)
        {
            ViewData["Title"] = "Thêm Nguyên Liệu";
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            await LoadFormLookupsAsync(token, ct);
            return View(new CreateIngredientClientRequest { IsActive = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateIngredientClientRequest request, CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    TempData["Error"] = "Vui lòng nhập tên nguyên liệu.";
                    return RedirectToAction(nameof(Create));
                }

                if (string.IsNullOrWhiteSpace(request.Unit))
                {
                    TempData["Error"] = "Vui lòng chọn đơn vị tính.";
                    return RedirectToAction(nameof(Create));
                }

                request.IsActive = IsActiveCheckedInForm();
                if (request.DefaultSupplierId is 0)
                    request.DefaultSupplierId = null;

                var result = await _client.CreateIngredientAsync(request, token, ct);
                TempData["Success"] = $"Đã thêm nguyên liệu «{result.Ingredient.Name}».";
                return RedirectToAction(nameof(Detail), new { id = result.Ingredient.Id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Create));
            }
        }

        public async Task<IActionResult> Edit(int id, CancellationToken ct = default)
        {
            ViewData["Title"] = "Sửa Nguyên Liệu";
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var response = await _client.GetIngredientAsync(id, token, ct);
                await LoadFormLookupsAsync(token, ct);
                ViewBag.IngredientId = id;
                return View(response.Ingredient);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateIngredientClientRequest request, CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    TempData["Error"] = "Vui lòng nhập tên nguyên liệu.";
                    return RedirectToAction(nameof(Edit), new { id });
                }

                if (string.IsNullOrWhiteSpace(request.Unit))
                {
                    TempData["Error"] = "Vui lòng chọn đơn vị tính.";
                    return RedirectToAction(nameof(Edit), new { id });
                }

                request.IsActive = IsActiveCheckedInForm();
                if (request.DefaultSupplierId is 0)
                    request.DefaultSupplierId = null;

                var result = await _client.UpdateIngredientAsync(id, request, token, ct);
                TempData["Success"] = $"Đã cập nhật nguyên liệu «{result.Ingredient.Name}».";
                return RedirectToAction(nameof(Detail), new { id });
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
                var result = await _client.DeleteIngredientAsync(id, token, ct);
                TempData["Success"] = result.Message;
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadFormLookupsAsync(string token, CancellationToken ct)
        {
            try
            {
                var partners = await _masterData.GetPartnersAsync(token, 1, 200, null, ct);
                ViewBag.Partners = partners.Items;
            }
            catch
            {
                ViewBag.Partners = new List<PartnerDto>();
            }
        }

        /// <summary>Checkbox chỉ gửi giá trị khi được tích; không dùng input hidden (tránh binder lấy false trước).</summary>
        private bool IsActiveCheckedInForm() =>
            Request.Form.TryGetValue("IsActive", out var values) &&
            values.Any(v => string.Equals(v, "true", StringComparison.OrdinalIgnoreCase));
    }
}
