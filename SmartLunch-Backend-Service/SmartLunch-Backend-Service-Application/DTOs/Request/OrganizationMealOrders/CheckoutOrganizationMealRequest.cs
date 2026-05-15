namespace SmartLunch.Backend.Service.Application.DTOs.Request.OrganizationMealOrders;

public sealed class CheckoutOrganizationMealRequest
{
    /// <summary>Mã nháy từ POST contract (Redis).</summary>
    public string DraftId { get; set; } = string.Empty;

    /// <summary>Thanh toán trước theo % giá trị đơn: 20, 25, 30, 35 hoặc 50.</summary>
    public int DepositPercent { get; set; }

    public string? ReturnUrl { get; set; }
    public string? CancelUrl { get; set; }
}
