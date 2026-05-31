using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khoa_Luan_KS_Web.Areas.Manager.Models;
using Khoa_Luan_KS_Web.Helpers;

namespace Khoa_Luan_KS_Web.Areas.Manager.Controllers;

[Area("Manager")]
[Authorize(Policy = "ManagerArea")]
public class OrderController : Controller
{
    private readonly Services.BackendMasterDataClient _masterDataClient;

    public OrderController(Services.BackendMasterDataClient masterDataClient)
    {
        _masterDataClient = masterDataClient;
    }

    public async Task<IActionResult> Index(
        int page = 1,
        int pageSize = 15,
        string? status = null,
        string? search = null,
        DateOnly? scheduledOn = null,
        CancellationToken ct = default)
    {
        var token = HttpContext.Session.GetString("access_token");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Auth", new { area = "" });

        page = page < 1 ? 1 : page;
        pageSize = pageSize switch
        {
            10 => 10,
            20 => 20,
            30 => 30,
            50 => 50,
            _ => 15
        };

        try
        {
            var orders = await _masterDataClient.GetOrdersAsync(
                token, page, pageSize, status, search, scheduledOn, paymentStatus: null, ct: ct);

            var effectivePageSize = orders.PageSize > 0 ? orders.PageSize : pageSize;
            var totalPages = effectivePageSize > 0
                ? Math.Max(1, (int)Math.Ceiling(orders.TotalCount / (double)effectivePageSize))
                : 1;
            var effectivePage = orders.Page > 0 ? orders.Page : page;

            if (orders.TotalCount > 0 && effectivePage > totalPages)
            {
                return RedirectToAction(nameof(Index), new
                {
                    page = totalPages,
                    pageSize = effectivePageSize,
                    status,
                    search,
                    scheduledOn = scheduledOn?.ToString("yyyy-MM-dd")
                });
            }

            var statsSource = await _masterDataClient.GetOrdersAsync(token, 1, 500, null, null, null, paymentStatus: null, ct: ct);
            var today = DateOnly.FromDateTime(DateTime.Now);

            var vm = new ManagerOrdersPageVm
            {
                Orders = orders,
                Page = effectivePage,
                PageSize = effectivePageSize,
                FilterStatus = status,
                Search = search,
                ScheduledOn = scheduledOn,
                CountPending = statsSource.Items.Count(o => o.Status.Equals("pending", StringComparison.OrdinalIgnoreCase)),
                CountConfirmed = statsSource.Items.Count(o =>
                    o.Status.Equals("confirmed", StringComparison.OrdinalIgnoreCase) ||
                    o.Status.Equals("preparing", StringComparison.OrdinalIgnoreCase)),
                CountPreparing = statsSource.Items.Count(o => o.Status.Equals("preparing", StringComparison.OrdinalIgnoreCase)),
                CountDelivered = statsSource.Items.Count(o => o.Status.Equals("delivered", StringComparison.OrdinalIgnoreCase)),
                CountDeliveredToday = statsSource.Items.Count(o =>
                    o.Status.Equals("delivered", StringComparison.OrdinalIgnoreCase) &&
                    DateOnly.FromDateTime(o.ScheduledDate) == today)
            };

            return View(vm);
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return View(new ManagerOrdersPageVm
            {
                Page = page,
                PageSize = pageSize,
                FilterStatus = status,
                Search = search,
                ScheduledOn = scheduledOn
            });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Detail(int id, CancellationToken ct)
    {
        var token = HttpContext.Session.GetString("access_token");
        if (string.IsNullOrEmpty(token))
            return Unauthorized();

        try
        {
            var res = await _masterDataClient.GetOrderAsync(id, token, ct);
            return Json(new { success = true, order = MapOrderRow(res.Order) });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string status, CancellationToken ct)
    {
        var token = HttpContext.Session.GetString("access_token");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Auth", new { area = "" });

        try
        {
            await _masterDataClient.UpdateOrderStatusAsync(id, status.Trim().ToLowerInvariant(), token, ct);
            TempData["Success"] = $"Đã cập nhật trạng thái đơn #{id} thành {OrderStatusHelper.ToDisplay(status)}.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        var page = int.TryParse(Request.Form["page"], out var p) ? p : 1;
        var pageSize = int.TryParse(Request.Form["pageSize"], out var ps) ? ps : 15;
        return RedirectToAction(nameof(Index), new
        {
            page,
            pageSize,
            status = Request.Form["filterStatus"].FirstOrDefault(),
            search = Request.Form["search"].FirstOrDefault(),
            scheduledOn = Request.Form["scheduledOn"].FirstOrDefault()
        });
    }

    private static object MapOrderRow(Services.OrderDetailClientDto o)
    {
        var vi = CultureInfo.GetCultureInfo("vi-VN");
        var totalQty = o.Items.Sum(i => i.Quantity);
        var mealSummary = o.Items.Count switch
        {
            0 => "—",
            1 => o.Items[0].DishName,
            _ => $"{o.Items[0].DishName} +{o.Items.Count - 1} món"
        };

        return new
        {
            id = o.Id,
            code = string.IsNullOrWhiteSpace(o.InvoiceCode) ? $"ĐH-{o.Id}" : o.InvoiceCode,
            customer = o.OrganizationName ?? "Khách hàng",
            address = o.ContractSummary?.ContractNumber != null
                ? $"Hợp đồng: {o.ContractSummary.ContractNumber}"
                : "—",
            type = mealSummary,
            quantity = totalQty,
            time = o.ScheduledDate.ToString("dd/MM/yyyy HH:mm", vi),
            orderDate = o.OrderDate.ToString("dd/MM/yyyy HH:mm", vi),
            total = o.TotalAmount.ToString("N0", vi) + " đ",
            subtotal = (o.SubtotalAmount ?? o.TotalAmount + o.DiscountAmount).ToString("N0", vi) + " đ",
            discount = o.DiscountAmount.ToString("N0", vi) + " đ",
            promotionName = o.AppliedPromotion?.PromotionName,
            promotionCode = o.AppliedPromotion?.PromotionCode,
            status = o.Status,
            statusLabel = OrderStatusHelper.ToDisplay(o.Status),
            statusClass = OrderStatusHelper.BadgeClass(o.Status),
            paymentStatus = o.PaymentStatus,
            paymentLabel = OrderStatusHelper.PaymentDisplay(o.PaymentStatus),
            paymentClass = OrderStatusHelper.PaymentBadgeClass(o.PaymentStatus),
            annexSigned = !string.IsNullOrEmpty(o.AnnexPdfUrl),
            annexPdfUrl = o.AnnexPdfUrl,
            items = o.Items.Select(i => new
            {
                dishName = i.DishName,
                quantity = i.Quantity,
                unitPrice = i.UnitPrice.ToString("N0", vi) + " đ",
                totalPrice = i.TotalPrice.ToString("N0", vi) + " đ"
            }).ToList(),
            editableStatuses = OrderStatusHelper.GetEditableStatuses(o.Status)
                .Select(s => new { value = s.Value, label = s.Label })
                .ToList()
        };
    }
}
