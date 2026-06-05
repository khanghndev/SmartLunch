namespace Khoa_Luan_KS_Web.Helpers;

public static class ContractWeeklySelectionDisplay
{
    public const string TrackWeekButtonLabel = "Theo dõi đơn hàng theo tuần";
    public const string SelectWeekButtonLabel = "Chọn món tuần này";
    public const string ManageContractButtonLabel = "Quản lý suất ăn theo tuần";

    public static string StatusLabel(string? status) => (status ?? "").Trim().ToLowerInvariant() switch
    {
        "pending" => "Chưa chọn món",
        "selected" => "Đã chọn món",
        "auto_filled" => "Hệ thống tự chọn",
        _ => status ?? "—",
    };

    public static string StatusBadgeClass(string? status) => (status ?? "").Trim().ToLowerInvariant() switch
    {
        "pending" => "bg-amber-100 text-amber-800",
        "selected" => "bg-emerald-100 text-emerald-800",
        "auto_filled" => "bg-sky-100 text-sky-800",
        _ => "bg-slate-100 text-slate-700",
    };

    public static bool IsWeeklyFulfillmentOrder(string? invoiceCode, int? contractId) =>
        contractId is > 0
        && !string.IsNullOrWhiteSpace(invoiceCode)
        && invoiceCode.StartsWith("Tuan-", StringComparison.OrdinalIgnoreCase);
}
