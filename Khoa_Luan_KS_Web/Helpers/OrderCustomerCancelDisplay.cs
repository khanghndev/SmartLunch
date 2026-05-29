namespace Khoa_Luan_KS_Web.Helpers;

/// <summary>Hiển thị nút hủy đơn — khớp quy tắc hủy đơn phía backend.</summary>
public static class OrderCustomerCancelDisplay
{
    public static bool CanCancel(string? status, string? paymentStatus, DateTime scheduledDate)
    {
        var s = (status ?? "").Trim().ToLowerInvariant();
        if (s != "pending")
            return false;

        var p = (paymentStatus ?? "").Trim().ToLowerInvariant();
        if (p is "deposit_paid" or "partial" or "paid")
            return false;

        return scheduledDate.Date >= DateTime.Today;
    }

    public static string? BlockHint(string? status, string? paymentStatus)
    {
        var s = (status ?? "").Trim().ToLowerInvariant();
        if (s == "cancelled")
            return "Đơn đã hủy";

        if (s is "delivered" or "confirmed" or "preparing")
            return "Liên hệ hỗ trợ để hủy";

        var p = (paymentStatus ?? "").Trim().ToLowerInvariant();
        if (p is "deposit_paid" or "partial" or "paid")
            return "Đã thanh toán — liên hệ hoàn tiền";

        return null;
    }
}
