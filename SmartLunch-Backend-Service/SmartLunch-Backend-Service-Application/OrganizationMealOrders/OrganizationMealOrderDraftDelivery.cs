namespace SmartLunch.Backend.Service.Application.OrganizationMealOrders;

public sealed class OrganizationMealOrderDraftDelivery
{
    public string RecipientName { get; set; } = string.Empty;
    public string RecipientPhone { get; set; } = string.Empty;
    public string RecipientEmail { get; set; } = string.Empty;
    public string DeliveryAddress { get; set; } = string.Empty;
    public string? DeliveryWardDistrict { get; set; }
    public string? DeliveryNotes { get; set; }
    public string? PreferredDeliveryTime { get; set; }
}
