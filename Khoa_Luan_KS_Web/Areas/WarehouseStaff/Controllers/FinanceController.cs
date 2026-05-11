using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khoa_Luan_KS_Web.Services;

namespace Khoa_Luan_KS_Web.Areas.WarehouseStaff.Controllers
{
    [Area("WarehouseStaff")]
    [Authorize(Policy = "ManagerArea")]
    public class FinanceController : Controller
    {
        private readonly BackendWarehouseClient _client;
        public FinanceController(BackendWarehouseClient client) { _client = client; }

        public async Task<IActionResult> SupplierPayables(bool onlyOutstanding = true, int? partnerId = null, CancellationToken ct = default)
        {
            ViewData["Title"] = "Công Nợ Phải Trả NCC";
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var response = await _client.GetSupplierPayablesAsync(token, onlyOutstanding, partnerId, ct);
                ViewBag.OnlyOutstanding = onlyOutstanding;
                ViewBag.PartnerId = partnerId;
                return View(response);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new SupplierPayablesClientResponse());
            }
        }
    }
}
