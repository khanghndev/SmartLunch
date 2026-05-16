namespace SmartLunch.Backend.Service.Application.Constants;

/// <summary>Values stored on Order.PaymentStatus.</summary>
public static class OrderPaymentStatus
{
    public const string Unpaid = "unpaid";
    /// <summary>Đã ký phụ lục, chờ khách thanh toán đặt cọc.</summary>
    public const string AwaitingPayment = "awaiting_payment";
    /// <summary>Đã thanh toán đặt cọc (chưa thanh toán hết giá trị đơn).</summary>
    public const string DepositPaid = "deposit_paid";
    public const string Partial = "partial";
    public const string Paid = "paid";
}
