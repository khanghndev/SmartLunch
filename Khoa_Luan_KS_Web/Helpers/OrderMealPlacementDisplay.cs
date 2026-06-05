using Khoa_Luan_KS_Web.Services;

namespace Khoa_Luan_KS_Web.Helpers;

/// <summary>
/// Nút «Đặt suất ăn» trên lịch sử đơn sau khi đặt cọc.
/// Period-Based → quản lý suất ăn theo tuần (chi tiết HĐ); Order-Based → xem chi tiết đơn.
/// </summary>
public static class OrderMealPlacementDisplay
{
    public const string ButtonLabel = "Đặt suất ăn";

    public static bool IsDepositPaid(string? paymentStatus)
    {
        var p = (paymentStatus ?? "").Trim().ToLowerInvariant();
        return p is "deposit_paid" or "partial";
    }

    public static bool IsBlockedStatus(string? status)
    {
        var s = (status ?? "").Trim().ToLowerInvariant();
        return s is "cancelled" or "delivered";
    }

    public static DateOnly ToScheduledDateOnly(DateTime scheduledDate) =>
        DateOnly.FromDateTime(scheduledDate.Date);

    public static bool IsPeriodBasedContract(OrderContractSummaryClientDto? contract) =>
        string.Equals(contract?.ContractType, "Period-Based", StringComparison.OrdinalIgnoreCase);

    public static bool IsOrderBasedContract(OrderContractSummaryClientDto? contract) =>
        string.Equals(contract?.ContractType, "Order-Based", StringComparison.OrdinalIgnoreCase);

    public static bool IsContractReadyForMealSelection(
        OrderContractSummaryClientDto? contract,
        string? annexPdfUrl)
    {
        if (contract?.IsDigitallySigned == true)
            return true;
        return !string.IsNullOrWhiteSpace(annexPdfUrl);
    }

    public static bool IsWithinContractPeriod(OrderContractSummaryClientDto? contract, DateOnly todayVietnam)
    {
        if (contract?.EndDate == null)
            return true;
        var end = DateOnly.FromDateTime(contract.EndDate.Value.Date);
        return todayVietnam <= end;
    }

    public static int ResolveContractId(OrderDetailClientDto order) =>
        order.ContractSummary?.Id ?? order.ContractId ?? 0;

    public static bool HasReachedScheduledDate(DateTime scheduledDate, DateOnly todayVietnam) =>
        todayVietnam >= ToScheduledDateOnly(scheduledDate);

    public static PlaceMealLink? TryGetPlaceMealLink(
        OrderDetailClientDto order,
        bool isOrganizationAccount,
        DateOnly? todayVietnam = null)
    {
        if (!isOrganizationAccount || order.Id <= 0)
            return null;

        if (!IsDepositPaid(order.PaymentStatus))
            return null;

        if (IsBlockedStatus(order.Status))
            return null;

        if (!IsContractReadyForMealSelection(order.ContractSummary, order.AnnexPdfUrl))
            return null;

        var today = todayVietnam ?? OrganizationMealContractDateRules.TodayVietnam();
        var contract = order.ContractSummary;
        var contractId = ResolveContractId(order);

        if (IsPeriodBasedContract(contract))
        {
            if (contractId <= 0)
                return null;
            if (!IsWithinContractPeriod(contract, today))
                return null;

            var openWeek = ResolveOpenWeekForPlacement(contract, today);
            if (!openWeek.HasValue)
                return null;

            return new PlaceMealLink(
                Controller: "OrganizationMealContractOrder",
                Action: "Detail",
                RouteValues: ContractDetailRoute(contractId),
                Title: ContractWeeklySelectionDisplay.ManageContractButtonLabel);
        }

        if (IsOrderBasedContract(contract))
        {
            return new PlaceMealLink(
                Controller: "Profile",
                Action: "OrderDetail",
                RouteValues: OrderDetailRoute(order.Id),
                Title: HasReachedScheduledDate(order.ScheduledDate, today)
                    ? $"Xem suất đã đặt — ngày giao {ToScheduledDateOnly(order.ScheduledDate):dd/MM/yyyy}"
                    : $"Xem suất đã đặt — giao dự kiến {ToScheduledDateOnly(order.ScheduledDate):dd/MM/yyyy}");
        }

        if (contract == null && !string.IsNullOrWhiteSpace(order.AnnexPdfUrl))
        {
            return new PlaceMealLink(
                Controller: "Profile",
                Action: "OrderDetail",
                RouteValues: OrderDetailRoute(order.Id),
                Title: $"Xem suất đã đặt — giao dự kiến {ToScheduledDateOnly(order.ScheduledDate):dd/MM/yyyy}");
        }

        return null;
    }

    public static bool CanShowPlaceMealButton(
        OrderDetailClientDto order,
        bool isOrganizationAccount,
        DateOnly? todayVietnam = null) =>
        TryGetPlaceMealLink(order, isOrganizationAccount, todayVietnam) != null;

    public static DateOnly? ResolveOpenWeekForPlacement(
        OrderContractSummaryClientDto? contract,
        DateOnly todayVietnam)
    {
        if (contract?.EndDate == null)
            return null;

        var contractStart = DateOnly.FromDateTime(contract.StartDate.Date);
        var contractEnd = DateOnly.FromDateTime(contract.EndDate.Value.Date);
        return OrganizationMealContractDateRules.ResolveOpenWeekMonday(
            todayVietnam, contractStart, contractEnd, Array.Empty<DateOnly>());
    }

    public static string? PlaceMealTitle(OrderDetailClientDto order, DateOnly? todayVietnam = null) =>
        TryGetPlaceMealLink(order, isOrganizationAccount: true, todayVietnam)?.Title;

    private static Dictionary<string, string> ContractDetailRoute(int contractId) =>
        new() { ["id"] = contractId.ToString() };

    private static Dictionary<string, string> OrderDetailRoute(int orderId) =>
        new() { ["id"] = orderId.ToString() };
}

public sealed record PlaceMealLink(
    string Controller,
    string Action,
    Dictionary<string, string> RouteValues,
    string Title);
