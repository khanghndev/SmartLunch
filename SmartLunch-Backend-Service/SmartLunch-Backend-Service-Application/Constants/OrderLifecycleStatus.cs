namespace SmartLunch.Backend.Service.Application.Constants;

/// <summary>
/// Order status values aligned with meal-order workflow (see entity Order.Status).
/// </summary>
public static class OrderLifecycleStatus
{
    public const string Pending = "pending";
    public const string Confirmed = "confirmed";
    public const string Preparing = "preparing";
    public const string Delivered = "delivered";
    public const string Cancelled = "cancelled";
}
