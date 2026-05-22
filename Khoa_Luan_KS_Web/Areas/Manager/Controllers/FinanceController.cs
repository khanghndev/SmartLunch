using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khoa_Luan_KS_Web.Services;

namespace Khoa_Luan_KS_Web.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Policy = "ManagerArea")]
    public class FinanceController : Controller
    {
        private readonly BackendMasterDataClient _masterDataClient;

        public FinanceController(BackendMasterDataClient masterDataClient)
        {
            _masterDataClient = masterDataClient;
        }

        public IActionResult Index() => View();

        /// <summary>
        /// Công nợ phải thu từ khách hàng doanh nghiệp (Organization).
        /// </summary>
        public async Task<IActionResult> Debts(
            int page = 1,
            int pageSize = 20,
            string? searchTerm = null,
            string? statusFilter = null,
            CancellationToken ct = default)
        {
            var accessToken = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(accessToken))
                return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var receivables = await _masterDataClient.GetOrganizationReceivablesAsync(
                    accessToken, onlyWithOutstanding: false, ct: ct);

                var units = await _masterDataClient.GetUnitsAsync(accessToken, 1, 500, ct: ct);
                var unitMap = units.Items.ToDictionary(u => u.Id);

                var rows = receivables.Lines.Select(line =>
                {
                    unitMap.TryGetValue(line.OrganizationId, out var unit);
                    return new DebtRowViewModel
                    {
                        OrganizationId = line.OrganizationId,
                        OrganizationName = string.IsNullOrWhiteSpace(line.OrganizationName)
                            ? (unit?.Name ?? $"Đơn vị #{line.OrganizationId}")
                            : line.OrganizationName,
                        ContactPerson = unit?.ContactPerson,
                        Phone = unit?.Phone,
                        OrderCount = line.OrderCount,
                        TotalBilled = line.TotalBilled,
                        TotalPaid = line.TotalPaid,
                        Outstanding = line.Outstanding,
                        UiStatus = UiStatus(line.Outstanding)
                    };
                }).ToList();

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    var term = searchTerm.Trim();
                    rows = rows.Where(r =>
                        r.OrganizationName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                        (r.ContactPerson?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (r.Phone?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false))
                        .ToList();
                }

                rows = ApplyStatusFilter(rows, statusFilter);

                var totalCount = rows.Count;
                var pageItems = rows
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                ViewData["SearchTerm"] = searchTerm;
                ViewData["StatusFilter"] = statusFilter ?? "all";
                ViewData["CurrentPage"] = page;
                ViewData["PageSize"] = pageSize;
                ViewData["GrandTotalOutstanding"] = rows.Where(r => r.Outstanding > 0.01m).Sum(r => r.Outstanding);

                return View(new DebtsPageViewModel
                {
                    Rows = pageItems,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize
                });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                ViewData["SearchTerm"] = searchTerm;
                ViewData["StatusFilter"] = statusFilter ?? "all";
                ViewData["CurrentPage"] = page;
                ViewData["PageSize"] = pageSize;
                ViewData["GrandTotalOutstanding"] = 0m;
                return View(new DebtsPageViewModel());
            }
        }

        [HttpGet]
        public async Task<IActionResult> PaymentHistory(int organizationId, CancellationToken ct = default)
        {
            var accessToken = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(accessToken))
                return Unauthorized(new { message = "Not authenticated" });

            try
            {
                var to = DateOnly.FromDateTime(DateTime.Today);
                var from = to.AddMonths(-6);
                var history = await _masterDataClient.GetPaymentHistoryAsync(
                    accessToken, from, to, "Customer", organizationId, 1, 50, ct);
                return Json(history);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private static List<DebtRowViewModel> ApplyStatusFilter(List<DebtRowViewModel> rows, string? statusFilter)
        {
            if (string.IsNullOrWhiteSpace(statusFilter) || statusFilter == "all")
                return rows;

            return statusFilter switch
            {
                "outstanding" => rows.Where(r => r.Outstanding > 0.01m).ToList(),
                "paid" => rows.Where(r => r.Outstanding <= 0.01m).ToList(),
                _ => rows
            };
        }

        internal static string UiStatus(decimal outstanding) =>
            outstanding <= 0.01m ? "Đã thanh toán" : "Còn nợ";
    }

    public sealed class DebtsPageViewModel
    {
        public List<DebtRowViewModel> Rows { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public sealed class DebtRowViewModel
    {
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public string? ContactPerson { get; set; }
        public string? Phone { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalBilled { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal Outstanding { get; set; }
        public string UiStatus { get; set; } = string.Empty;
    }
}
