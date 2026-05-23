using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khoa_Luan_KS_Web.Services;

namespace Khoa_Luan_KS_Web.Areas.Manager.Controllers;

[Area("Manager")]
[Authorize(Policy = "ManagerArea")]
public class ReportController : Controller
{
    private readonly BackendMasterDataClient _masterDataClient;
    private readonly ReportExcelExportService _excelExport;

    public ReportController(BackendMasterDataClient masterDataClient, ReportExcelExportService excelExport)
    {
        _masterDataClient = masterDataClient;
        _excelExport = excelExport;
    }

    public IActionResult Index() => View();

    public async Task<IActionResult> Meals(
        DateOnly? from,
        DateOnly? to,
        int? organizationId,
        CancellationToken ct = default)
    {
        var token = HttpContext.Session.GetString("access_token");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Auth", new { area = "" });

        var today = DateOnly.FromDateTime(DateTime.Today);
        var dateFrom = from ?? new DateOnly(today.Year, today.Month, 1);
        var dateTo = to ?? today;
        if (dateTo < dateFrom)
            (dateFrom, dateTo) = (dateTo, dateFrom);

        var vm = new MealReportViewModel
        {
            From = dateFrom,
            To = dateTo,
            OrganizationId = organizationId,
        };

        try
        {
            var units = await _masterDataClient.GetUnitsAsync(token, 1, 500, isActive: true, ct: ct);
            vm.Organizations = units.Items.Select(u => new ReportOrgOption(u.Id, u.Name)).ToList();

            var summary = await _masterDataClient.GetMealStatisticsAsync(token, dateFrom, dateTo, organizationId, ct);
            var detailed = await _masterDataClient.GetDetailedMealStatisticsAsync(token, dateFrom, dateTo, organizationId, ct);
            vm.SummaryRows = summary.Data;
            vm.DetailedRows = detailed.Data;

            if (organizationId.HasValue)
                vm.OrganizationLabel = vm.Organizations.FirstOrDefault(o => o.Id == organizationId.Value)?.Name;
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return View(vm);
    }

    public async Task<IActionResult> ExportMealsSummary(
        DateOnly? from,
        DateOnly? to,
        int? organizationId,
        CancellationToken ct = default)
    {
        if (!TryGetToken(out var token, out var redirect)) return redirect!;
        var (dateFrom, dateTo) = ResolveRange(from, to);
        var data = await _masterDataClient.GetMealStatisticsAsync(token, dateFrom, dateTo, organizationId, ct);
        var orgLabel = await ResolveOrgLabel(token, organizationId, ct);
        var bytes = _excelExport.ExportMealSummary(data.Data, dateFrom, dateTo, orgLabel);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Bao_cao_suat_an_{dateFrom:yyyyMMdd}_{dateTo:yyyyMMdd}.xlsx");
    }

    public async Task<IActionResult> ExportMealsDetailed(
        DateOnly? from,
        DateOnly? to,
        int? organizationId,
        CancellationToken ct = default)
    {
        if (!TryGetToken(out var token, out var redirect)) return redirect!;
        var (dateFrom, dateTo) = ResolveRange(from, to);
        var data = await _masterDataClient.GetDetailedMealStatisticsAsync(token, dateFrom, dateTo, organizationId, ct);
        var orgLabel = await ResolveOrgLabel(token, organizationId, ct);
        var bytes = _excelExport.ExportMealDetailed(data.Data, dateFrom, dateTo, orgLabel);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Chi_tiet_mon_an_{dateFrom:yyyyMMdd}_{dateTo:yyyyMMdd}.xlsx");
    }

    public async Task<IActionResult> ExportFinance(
        string period = "current_month",
        CancellationToken ct = default)
    {
        if (!TryGetToken(out var token, out var redirect)) return redirect!;
        var (from, to, label) = ResolveFinancePeriod(period);
        var cashflow = await _masterDataClient.GetCashflowSummaryAsync(token, from, to, "Month", ct);
        var bytes = _excelExport.ExportCashflow(cashflow, label);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Bao_cao_tai_chinh_{from:yyyyMMdd}_{to:yyyyMMdd}.xlsx");
    }

    public async Task<IActionResult> ExportPayments(
        DateOnly? from,
        DateOnly? to,
        CancellationToken ct = default)
    {
        if (!TryGetToken(out var token, out var redirect)) return redirect!;
        var (dateFrom, dateTo) = ResolveRange(from, to);
        var history = await _masterDataClient.GetPaymentHistoryAsync(
            token, dateFrom, dateTo, "All", page: 1, pageSize: 5000, ct: ct);
        var bytes = _excelExport.ExportPaymentHistory(history.Entries, dateFrom, dateTo);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Lich_su_thanh_toan_{dateFrom:yyyyMMdd}_{dateTo:yyyyMMdd}.xlsx");
    }

    private bool TryGetToken(out string token, out IActionResult redirect)
    {
        token = HttpContext.Session.GetString("access_token") ?? "";
        if (!string.IsNullOrEmpty(token))
        {
            redirect = null!;
            return true;
        }
        redirect = RedirectToAction("Login", "Auth", new { area = "" });
        return false;
    }

    private static (DateOnly from, DateOnly to) ResolveRange(DateOnly? from, DateOnly? to)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var dateFrom = from ?? new DateOnly(today.Year, today.Month, 1);
        var dateTo = to ?? today;
        if (dateTo < dateFrom)
            (dateFrom, dateTo) = (dateTo, dateFrom);
        return (dateFrom, dateTo);
    }

    private static (DateOnly from, DateOnly to, string label) ResolveFinancePeriod(string period)
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
            _ => (
                new DateOnly(today.Year, today.Month, 1),
                today,
                $"Tháng {today.Month}/{today.Year}")
        };
    }

    private async Task<string?> ResolveOrgLabel(string token, int? organizationId, CancellationToken ct)
    {
        if (!organizationId.HasValue) return null;
        try
        {
            var u = await _masterDataClient.GetUnitAsync(organizationId.Value, token, ct);
            return u.Unit?.Name;
        }
        catch
        {
            return null;
        }
    }
}

public sealed record ReportOrgOption(int Id, string Name);

public sealed class MealReportViewModel
{
    public DateOnly From { get; set; }
    public DateOnly To { get; set; }
    public int? OrganizationId { get; set; }
    public string? OrganizationLabel { get; set; }
    public List<ReportOrgOption> Organizations { get; set; } = new();
    public List<MealStatisticItemClientDto> SummaryRows { get; set; } = new();
    public List<DetailedMealItemClientDto> DetailedRows { get; set; } = new();

    public int TotalMeals => SummaryRows.Sum(x => x.TotalMeals);
    public decimal TotalRevenue => SummaryRows.Sum(x => x.TotalAmount);
}
