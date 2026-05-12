using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Khoa_Luan_KS_Web.Services;

namespace Khoa_Luan_KS_Web.Areas.WarehouseStaff.Controllers
{
    [Area("WarehouseStaff")]
    [Authorize(Policy = "WarehouseStaffArea")]
    public class RequisitionController : Controller
    {
        private readonly BackendWarehouseClient _client;
        public RequisitionController(BackendWarehouseClient client) { _client = client; }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 20, CancellationToken ct = default)
        {
            ViewData["Title"] = "Danh Sách Phiếu Đề Xuất";
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var response = await _client.GetIntakeProposalsAsync(token, page, pageSize, ct);
                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                return View(response);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new GetIntakeProposalsClientResponse());
            }
        }

        public async Task<IActionResult> Create(CancellationToken ct = default)
        {
            ViewData["Title"] = "Tạo Phiếu Đề Xuất";
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                // Pull active ingredients to allow selection
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
        public async Task<IActionResult> Create(string? headerNote, string linesJson, CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var lines = JsonSerializer.Deserialize<List<CreateIntakeProposalLineClientRequest>>(linesJson ?? "[]",
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

                lines = lines.Where(l => l.IngredientId > 0 && l.Quantity > 0).ToList();
                if (lines.Count == 0)
                {
                    TempData["Error"] = "Vui lòng thêm ít nhất một dòng nguyên liệu hợp lệ.";
                    return RedirectToAction(nameof(Create));
                }

                var request = new CreateIntakeProposalClientRequest
                {
                    HeaderNote = headerNote,
                    Lines = lines
                };

                var created = await _client.CreateIntakeProposalAsync(request, token, ct);
                TempData["Success"] = $"Đã tạo phiếu đề xuất #{created.Proposal.ProposalCode} thành công. Đang chờ Manager duyệt.";
                return RedirectToAction(nameof(Detail), new { id = created.Proposal.Id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Create));
            }
        }

        public async Task<IActionResult> Detail(int id, CancellationToken ct = default)
        {
            ViewData["Title"] = "Chi Tiết Phiếu Đề Xuất";
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var detail = await _client.GetIntakeProposalAsync(id, token, ct);
                return View(detail);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> History(int page = 1, int pageSize = 20, CancellationToken ct = default)
        {
            ViewData["Title"] = "Lịch Sử Duyệt Phiếu";
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var response = await _client.GetIntakeProposalReviewHistoryAsync(token, page, pageSize, ct);
                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                return View(response);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new GetIntakeReviewHistoryClientResponse());
            }
        }
    }
}
