namespace Khoa_Luan_KS_Web.Helpers;

public static class OrderPaymentStatusDisplay
{
    public static string Label(string? status) => (status ?? "").Trim().ToLowerInvariant() switch
    {
        "unpaid" => "Chưa thanh toán",
        "awaiting_payment" => "Đang chờ thanh toán",
        "deposit_paid" => "Đã đặt cọc",
        "partial" => "Đã đặt cọc",
        "paid" => "Đã thanh toán đủ",
        _ => status ?? "—",
    };

    public static string BadgeClass(string? status) => (status ?? "").Trim().ToLowerInvariant() switch
    {
        "awaiting_payment" => "bg-amber-100 text-amber-900",
        "deposit_paid" or "partial" => "bg-emerald-100 text-emerald-800",
        "paid" => "bg-emerald-100 text-emerald-800",
        "unpaid" => "bg-slate-100 text-slate-700",
        _ => "bg-slate-100 text-slate-600",
    };

    public static bool CanPayDeposit(string? status, string? annexPdfUrl = null)
    {
        if (string.Equals(status, "awaiting_payment", StringComparison.OrdinalIgnoreCase))
            return true;

        return !string.IsNullOrWhiteSpace(annexPdfUrl)
            && string.Equals(status, "unpaid", StringComparison.OrdinalIgnoreCase);
    }
}
