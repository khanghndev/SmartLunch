namespace Khoa_Luan_KS_Web.Helpers;

public static class OrderStatusHelper
{
    public static string ToDisplay(string? status)
    {
        var s = (status ?? "").Trim().ToLowerInvariant();
        return s switch
        {
            "pending" => "Chờ xác nhận",
            "confirmed" => "Đã xác nhận",
            "preparing" => "Đang chuẩn bị",
            "delivered" => "Đã giao hàng",
            "cancelled" => "Đã hủy",
            _ => string.IsNullOrWhiteSpace(status) ? "—" : status!
        };
    }

    public static string BadgeClass(string? status)
    {
        var s = (status ?? "").Trim().ToLowerInvariant();
        return s switch
        {
            "pending" => "bg-slate-100 text-slate-600 border-slate-200",
            "confirmed" => "bg-yellow-50 text-yellow-700 border-yellow-200",
            "preparing" => "bg-amber-50 text-amber-700 border-amber-200",
            "delivered" => "bg-green-50 text-green-700 border-green-200",
            "cancelled" => "bg-red-50 text-red-600 border-red-200",
            _ => "bg-slate-100 text-slate-500 border-slate-200"
        };
    }

    public static string PaymentBadgeClass(string? payment)
    {
        var p = (payment ?? "").Trim().ToLowerInvariant();
        return p switch
        {
            "paid" => "bg-green-50 text-green-700",
            "partial" => "bg-amber-50 text-amber-700",
            _ => "bg-slate-100 text-slate-600"
        };
    }

    public static string PaymentDisplay(string? payment)
    {
        var p = (payment ?? "").Trim().ToLowerInvariant();
        return p switch
        {
            "paid" => "Đã thanh toán",
            "partial" => "Thanh toán một phần",
            "unpaid" => "Chưa thanh toán",
            _ => payment ?? "—"
        };
    }

    /// <summary>Trạng thái tiếp theo hợp lệ (BE: pending → confirmed → delivered; preparing có thể nhảy confirmed/delivered).</summary>
    public static IReadOnlyList<(string Value, string Label)> GetEditableStatuses(string? current)
    {
        var c = (current ?? "").Trim().ToLowerInvariant();
        return c switch
        {
            "pending" => [("confirmed", "Xác nhận đơn")],
            "confirmed" => [("delivered", "Đánh dấu đã giao hàng")],
            "preparing" =>
            [
                ("confirmed", "Xác nhận đơn"),
                ("delivered", "Đánh dấu đã giao hàng")
            ],
            _ => []
        };
    }
}
