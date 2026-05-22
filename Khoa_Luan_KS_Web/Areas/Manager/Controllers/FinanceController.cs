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

        /// <summary>
        /// Báo cáo tài chính: tổng hợp thu chi + biểu đồ + giao dịch gần đây.
        /// </summary>
        public async Task<IActionResult> Index(string period = "current_month", CancellationToken ct = default)
        {
            var accessToken = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(accessToken))
                return RedirectToAction("Login", "Auth", new { area = "" });

            var periodKey = string.IsNullOrWhiteSpace(period) ? "current_month" : period;
            try
            {
                var (from, to, label) = ResolvePeriod(periodKey);
                var prev = ResolvePreviousPeriod(from, to);

                var cashflow = await _masterDataClient.GetCashflowSummaryAsync(
                    accessToken, from, to, "Month", ct);
                GetCashflowSummaryClientResponse? previous = null;
                try
                {
                    previous = await _masterDataClient.GetCashflowSummaryAsync(
                        accessToken, prev.from, prev.to, "Month", ct);
                }
                catch
                {
                    // Kỳ trước có thể không có dữ liệu
                }

                var chartFrom = to.AddMonths(-5);
                if (chartFrom < from) chartFrom = from;
                var chartCashflow = await _masterDataClient.GetCashflowSummaryAsync(
                    accessToken, chartFrom, to, "Month", ct);

                var historyTo = DateOnly.FromDateTime(DateTime.Today);
                var historyFrom = historyTo.AddDays(-30);
                var history = await _masterDataClient.GetPaymentHistoryAsync(
                    accessToken, historyFrom, historyTo, "All", page: 1, pageSize: 15, ct: ct);

                var vm = FinanceIndexViewModel.Build(
                    periodKey, label, from, to, cashflow, previous, chartCashflow, history.Entries);
                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(FinanceIndexViewModel.Empty(periodKey));
            }
        }

        private static (DateOnly from, DateOnly to, string label) ResolvePeriod(string period)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            return period switch
            {
                "last_month" => (
                    new DateOnly(today.Year, today.Month, 1).AddMonths(-1),
                    new DateOnly(today.Year, today.Month, 1).AddDays(-1),
                    "Tháng trước"),
                "last_6_months" => (
                    new DateOnly(today.Year, today.Month, 1).AddMonths(-5),
                    today,
                    "6 tháng gần nhất"),
                "q1" => (
                    new DateOnly(today.Year, 1, 1),
                    today.Month > 3 ? new DateOnly(today.Year, 3, 31) : today,
                    $"Quý 1 / {today.Year}"),
                _ => (
                    new DateOnly(today.Year, today.Month, 1),
                    today,
                    $"Tháng {today.Month} / {today.Year}")
            };
        }

        private static (DateOnly from, DateOnly to) ResolvePreviousPeriod(DateOnly from, DateOnly to)
        {
            var days = to.DayNumber - from.DayNumber + 1;
            var prevTo = from.AddDays(-1);
            var prevFrom = prevTo.AddDays(-(days - 1));
            return (prevFrom, prevTo);
        }

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
                    accessToken, from, to, "Customer", organizationId: organizationId, page: 1, pageSize: 50, ct: ct);
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

    public sealed class FinanceIndexViewModel
    {
        public string PeriodKey { get; set; } = "current_month";
        public string PeriodLabel { get; set; } = string.Empty;
        public DateOnly From { get; set; }
        public DateOnly To { get; set; }
        public decimal Revenue { get; set; }
        public decimal Expense { get; set; }
        public decimal Profit { get; set; }
        public decimal ProfitMarginPercent { get; set; }
        public decimal? RevenueChangePercent { get; set; }
        public decimal? ExpenseChangePercent { get; set; }
        public List<CashflowBucketClientDto> ChartBuckets { get; set; } = new();
        public List<FinanceTransactionRow> RecentTransactions { get; set; } = new();

        public static FinanceIndexViewModel Build(
            string periodKey,
            string periodLabel,
            DateOnly from,
            DateOnly to,
            GetCashflowSummaryClientResponse cashflow,
            GetCashflowSummaryClientResponse? previous,
            GetCashflowSummaryClientResponse chartCashflow,
            List<PaymentHistoryEntryClientDto> historyEntries)
        {
            var revenue = cashflow.TotalCollectedPayments + cashflow.TotalTransactionIncome;
            var expense = cashflow.TotalTransactionExpense;
            var profit = revenue - expense;
            var margin = revenue > 0 ? Math.Round(profit / revenue * 100, 1) : 0;

            decimal? revChange = null;
            decimal? expChange = null;
            if (previous != null)
            {
                var prevRev = previous.TotalCollectedPayments + previous.TotalTransactionIncome;
                var prevExp = previous.TotalTransactionExpense;
                if (prevRev > 0) revChange = Math.Round((revenue - prevRev) / prevRev * 100, 1);
                if (prevExp > 0) expChange = Math.Round((expense - prevExp) / prevExp * 100, 1);
            }

            var transactions = historyEntries
                .OrderByDescending(e => e.PaymentDate)
                .Select(e => new FinanceTransactionRow
                {
                    IsIncome = string.Equals(e.Source, "customer", StringComparison.OrdinalIgnoreCase),
                    Title = e.Source == "customer"
                        ? (e.OrderId.HasValue ? $"Thanh toán đơn #{e.OrderId}" : "Thu từ khách hàng")
                        : (e.ContractNumber != null ? $"Chi NCC · {e.ContractNumber}" : "Chi trả nhà cung cấp"),
                    Subtitle = e.OrganizationName ?? e.PartnerLegalName ?? e.Method,
                    Amount = e.Amount,
                    PaymentDate = e.PaymentDate,
                    Status = e.Status
                })
                .ToList();

            return new FinanceIndexViewModel
            {
                PeriodKey = periodKey,
                PeriodLabel = periodLabel,
                From = from,
                To = to,
                Revenue = revenue,
                Expense = expense,
                Profit = profit,
                ProfitMarginPercent = margin,
                RevenueChangePercent = revChange,
                ExpenseChangePercent = expChange,
                ChartBuckets = chartCashflow.Buckets,
                RecentTransactions = transactions
            };
        }

        public static FinanceIndexViewModel Empty(string period) => new()
        {
            PeriodKey = period,
            PeriodLabel = "—",
            From = DateOnly.FromDateTime(DateTime.Today),
            To = DateOnly.FromDateTime(DateTime.Today)
        };
    }

    public sealed class FinanceTransactionRow
    {
        public bool IsIncome { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
