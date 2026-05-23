namespace SmartLunch.Backend.Service.Application.DTOs.Request.OrganizationMealOrders;

/// <summary>Thông tin giao hàng & người nhận khi đặt suất đơn vị.</summary>
public sealed class OrganizationMealDeliveryRequest
{
    public string RecipientName { get; set; } = string.Empty;
    public string RecipientPhone { get; set; } = string.Empty;
    public string RecipientEmail { get; set; } = string.Empty;
    public string DeliveryAddress { get; set; } = string.Empty;
    public string? DeliveryWardDistrict { get; set; }
    public string? DeliveryNotes { get; set; }

    /// <summary>Giờ giao mong muốn, định dạng HH:mm (tùy chọn).</summary>
    public string? PreferredDeliveryTime { get; set; }
}
