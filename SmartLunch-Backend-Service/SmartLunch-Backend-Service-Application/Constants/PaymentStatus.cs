namespace SmartLunch.Backend.Service.Application.Constants;

public static class PaymentStatus
{
    public const string Pending = "pending";
    public const string Paid = "paid";
    public const string Refunded = "refunded";
}

public static class PaymentMethod
{
    public const string ComplaintRefund = "complaint_refund";
}

public static class TransactionCategory
{
    public const string ComplaintRefund = "complaint_refund";
}
