namespace SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealOrders;

public sealed class InitiateOrganizationMealPaymentResponse
{
    public int OrderId { get; set; }
    public int DepositAmountVnd { get; set; }
    public string? CheckoutUrl { get; set; }
    public string? QrCode { get; set; }
    public string? PayOsStatus { get; set; }
    public string? PayOsMessage { get; set; }

    /// <summary>PayOS đã PAID — hệ thống đã đồng bộ trạng thái đơn.</summary>
    public bool AlreadyPaidSynced { get; set; }
}
