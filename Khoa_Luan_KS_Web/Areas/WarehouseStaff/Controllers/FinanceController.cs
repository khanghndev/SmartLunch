using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khoa_Luan_KS_Web.Services;

namespace Khoa_Luan_KS_Web.Areas.WarehouseStaff.Controllers
{
    [Area("WarehouseStaff")]
    [Authorize(Policy = "WarehouseStaffArea")]
    public class FinanceController : Controller
    {
        private readonly BackendWarehouseClient _client;
        private readonly BackendMasterDataClient _masterData;

        public FinanceController(BackendWarehouseClient client, BackendMasterDataClient masterData)
        {
            _client = client;
            _masterData = masterData;
        }

        public async Task<IActionResult> SupplierPayables(
            bool onlyOutstanding = true,
            int? partnerId = null,
            CancellationToken ct = default)
        {
            ViewData["Title"] = "Công Nợ Phải Trả NCC";
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var partners = await _masterData.GetPartnersAsync(token, 1, 300, null, ct);
                ViewBag.Partners = partners.Items;
            }
            catch
            {
                ViewBag.Partners = new List<PartnerDto>();
            }

            try
            {
                var response = await _client.GetSupplierPayablesAsync(token, onlyOutstanding, partnerId, ct);
                ViewBag.OnlyOutstanding = onlyOutstanding;
                ViewBag.PartnerId = partnerId;

                var historyTo = DateOnly.FromDateTime(DateTime.Today);
                var historyFrom = historyTo.AddMonths(-3);
                try
                {
                    var history = await _client.GetSupplierPaymentHistoryAsync(
                        token, historyFrom, historyTo, partnerId, page: 1, pageSize: 50, ct: ct);
                    ViewBag.PaymentHistory = history.Entries;
                }
                catch
                {
                    ViewBag.PaymentHistory = new List<PaymentHistoryEntryClientDto>();
                }

                return View(response);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                ViewBag.OnlyOutstanding = onlyOutstanding;
                ViewBag.PartnerId = partnerId;
                ViewBag.PaymentHistory = new List<PaymentHistoryEntryClientDto>();
                return View(new SupplierPayablesClientResponse());
            }
        }

        [HttpGet]
        public async Task<IActionResult> SupplierContracts(int partnerId, CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return Unauthorized(new { message = "Not authenticated" });
            try
            {
                var data = await _client.GetSupplierContractsForPaymentAsync(partnerId, token, ct);
                return Json(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordSupplierPayment(
            [FromBody] CreateSupplierPaymentClientRequest request,
            CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return Unauthorized(new { message = "Not authenticated" });
            try
            {
                if (request.PaymentDate == default)
                    request.PaymentDate = DateTime.Today;
                var result = await _client.CreateSupplierPaymentAsync(request, token, ct);
                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
