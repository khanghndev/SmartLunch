using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Khoa_Luan_KS_Web.Services;

namespace Khoa_Luan_KS_Web.Areas.WarehouseStaff.Controllers
{
    [Area("WarehouseStaff")]
    [Authorize(Policy = "WarehouseStaffArea")]
    public class InternalIssueController : Controller
    {
        private readonly BackendWarehouseClient _client;
        public InternalIssueController(BackendWarehouseClient client) { _client = client; }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 20, DateOnly? from = null, DateOnly? to = null, CancellationToken ct = default)
        {
            ViewData["Title"] = "Phiếu Xuất Kho Nội Bộ";
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var response = await _client.GetInternalIssuesAsync(token, page, pageSize, from, to, ct);
                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.FromDate = from;
                ViewBag.ToDate = to;
                return View(response);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new GetInternalIssuesClientResponse());
            }
        }

        public async Task<IActionResult> Create(CancellationToken ct = default)
        {
            ViewData["Title"] = "Tạo Phiếu Xuất Kho Nội Bộ";
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var ingredients = await _client.GetIngredientsAsync(token, 1, 500, isActive: true, ct: ct);
                ViewBag.Ingredients = ingredients.Items;
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                ViewBag.Ingredients = new List<IngredientClientDto>();
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string? reason, string linesJson, CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var lines = JsonSerializer.Deserialize<List<CreateInternalIssueLineClientRequest>>(linesJson ?? "[]",
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

                lines = lines.Where(l => l.IngredientId > 0 && l.Quantity > 0).ToList();
                if (lines.Count == 0)
                {
                    TempData["Error"] = "Vui lòng thêm ít nhất một dòng nguyên liệu hợp lệ.";
                    return RedirectToAction(nameof(Create));
                }

                var request = new CreateInternalIssueClientRequest
                {
                    IssuedAtUtc = DateTime.UtcNow,
                    Reason = reason,
                    Lines = lines
                };

                var result = await _client.CreateInternalIssueAsync(request, token, ct);
                TempData["Success"] = $"Đã tạo phiếu xuất kho nội bộ #{result.Issue.IssueCode}. Tồn kho đã được trừ tự động.";
                return RedirectToAction(nameof(Detail), new { id = result.Issue.Id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Create));
            }
        }

        public async Task<IActionResult> Detail(int id, CancellationToken ct = default)
        {
            ViewData["Title"] = "Chi Tiết Phiếu Xuất Kho";
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var detail = await _client.GetInternalIssueAsync(id, token, ct);
                return View(detail);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
