using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khoa_Luan_KS_Web.Services;

namespace Khoa_Luan_KS_Web.Areas.WarehouseStaff.Controllers
{
    [Area("WarehouseStaff")]
    [Authorize(Policy = "WarehouseStaffArea")]
    public class WarehouseController : Controller
    {
        private readonly BackendWarehouseClient _client;
        public WarehouseController(BackendWarehouseClient client) { _client = client; }

        public async Task<IActionResult> Receive(CancellationToken ct = default)
        {
            ViewData["Title"] = "Nhập Kho Thực Tế";
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            var list = new List<IntakeProposalDetailClientDto>();
            try
            {
                var summary = await _client.GetIntakeProposalsAsync(token, 1, 100, ct);
                var approvedReady = summary.Data
                    .Where(p => string.Equals(p.Status, "approved", StringComparison.OrdinalIgnoreCase) && !p.HasActualReceipt)
                    .ToList();

                foreach (var p in approvedReady)
                {
                    try
                    {
                        var detail = await _client.GetIntakeProposalAsync(p.Id, token, ct);
                        list.Add(detail);
                    }
                    catch { /* skip a single failing proposal */ }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return View(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmReceive(int proposalId, bool confirmStandard, string? note, CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            if (!confirmStandard)
            {
                TempData["Error"] = "Bạn cần xác nhận nguyên liệu đạt chuẩn trước khi nhập kho.";
                return RedirectToAction(nameof(Receive));
            }

            try
            {
                var request = new CreateActualReceiptClientRequest
                {
                    ConfirmIngredientsMeetStandard = true,
                    ReceivedAtUtc = DateTime.UtcNow,
                    Note = note
                };
                var result = await _client.CreateActualReceiptAsync(proposalId, request, token, ct);
                TempData["Success"] = $"Đã nhập kho thực tế thành công. Mã phiếu nhập: {result.Receipt.ReceiptCode}";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Receive));
        }
    }
}
