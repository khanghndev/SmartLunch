using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khoa_Luan_KS_Web.Services;

namespace Khoa_Luan_KS_Web.Areas.WarehouseStaff.Controllers
{
    [Area("WarehouseStaff")]
    [Authorize(Policy = "ManagerArea")]
    public class HomeController : Controller
    {
        private readonly BackendWarehouseClient _client;

        public HomeController(BackendWarehouseClient client)
        {
            _client = client;
        }

        public async Task<IActionResult> Index(CancellationToken ct = default)
        {
            ViewData["Title"] = "Tổng quan Kho";

            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            var vm = new WarehouseDashboardViewModel();

            try
            {
                vm.LowStockAlerts = (await _client.GetLowStockAlertsAsync(token, ct)).Alerts;
            }
            catch (Exception ex) { TempData["Error"] = ex.Message; }

            try
            {
                var proposals = await _client.GetIntakeProposalsAsync(token, 1, 10, ct);
                vm.RecentProposals = proposals.Data;
                vm.PendingCount = proposals.Data.Count(p =>
                    string.Equals(p.Status, "submitted", StringComparison.OrdinalIgnoreCase));
                vm.ApprovedReadyCount = proposals.Data.Count(p =>
                    string.Equals(p.Status, "approved", StringComparison.OrdinalIgnoreCase)
                    && !p.HasActualReceipt);
            }
            catch (Exception ex) { TempData["Error"] = ex.Message; }

            try
            {
                vm.TotalIngredients = (await _client.GetIngredientsAsync(token, 1, 1, isActive: true, ct: ct)).TotalCount;
            }
            catch { /* ignore */ }

            try
            {
                vm.RecentIssues = (await _client.GetInternalIssuesAsync(token, 1, 5, ct: ct)).Data;
            }
            catch { /* ignore */ }

            return View(vm);
        }
    }

    public class WarehouseDashboardViewModel
    {
        public List<LowStockAlertClientDto> LowStockAlerts { get; set; } = new();
        public List<IntakeProposalSummaryClientDto> RecentProposals { get; set; } = new();
        public List<InternalIssueSummaryClientDto> RecentIssues { get; set; } = new();
        public int TotalIngredients { get; set; }
        public int PendingCount { get; set; }
        public int ApprovedReadyCount { get; set; }
    }
}
