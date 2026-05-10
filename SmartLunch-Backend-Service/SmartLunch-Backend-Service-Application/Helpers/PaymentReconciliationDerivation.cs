using SmartLunch.Backend.Service.Application.Constants;

namespace SmartLunch.Backend.Service.Application.Helpers;

public static class PaymentReconciliationDerivation
{
    public const string DerivedOverpaid = "overpaid";

    public static string DerivePaymentStatus(decimal orderTotal, decimal paidAmount)
    {
        if (paidAmount <= 0)
            return OrderPaymentStatus.Unpaid;
        if (paidAmount < orderTotal)
            return OrderPaymentStatus.Partial;
        if (paidAmount == orderTotal)
            return OrderPaymentStatus.Paid;
        return DerivedOverpaid;
    }

    public static string? BuildIssue(
        bool isAligned,
        string recorded,
        string derived,
        decimal pendingAmount,
        decimal difference)
    {
        if (!isAligned)
        {
            return
                $"Trạng thái ghi trên đơn ({recorded}) không khớp với tổng thanh toán paid ({derived}); chênh lệch: {difference}.";
        }

        if (pendingAmount > 0 && string.Equals(recorded, OrderPaymentStatus.Paid, StringComparison.Ordinal))
        {
            return "Đơn đánh dấu đã thanh toán nhưng vẫn còn khoản thanh toán ở trạng thái pending.";
        }

        return null;
    }
}
