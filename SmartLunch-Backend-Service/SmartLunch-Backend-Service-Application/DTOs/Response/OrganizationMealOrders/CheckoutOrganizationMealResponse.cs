using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;

namespace SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealOrders;

public sealed class CheckoutOrganizationMealResponse
{
    public GetOrderResponse Order { get; set; } = new();
    public int DepositPercent { get; set; }
    public int DepositAmountVnd { get; set; }
    public string? CheckoutUrl { get; set; }
    public string? QrCode { get; set; }
    public string? PayOsStatus { get; set; }
    public string? PayOsMessage { get; set; }
}
