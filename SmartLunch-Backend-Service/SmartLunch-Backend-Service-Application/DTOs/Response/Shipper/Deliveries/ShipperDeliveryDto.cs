namespace SmartLunch.Backend.Service.Application.DTOs.Response.Shipper.Deliveries;

public class ShipperDeliveryListItemDto
{
    public int DeliveryId { get; set; }
    public int OrderId { get; set; }
    public string DeliveryAddress { get; set; } = string.Empty;
    public string DeliveryStatus { get; set; } = string.Empty;
    public DateTime ScheduledDateUtc { get; set; }
    public int MealCount { get; set; }
}

public class ShipperDeliveryDetailDto
{
    public int DeliveryId { get; set; }
    public int OrderId { get; set; }
    public string DeliveryAddress { get; set; } = string.Empty;
    public string DeliveryStatus { get; set; } = string.Empty;
    public DateTime ScheduledDateUtc { get; set; }
    public int MealCount { get; set; }
    public DateTime? DeliveredAtUtc { get; set; }
    public string? ProofImageUrl { get; set; }
    public DateTime? ProofCapturedAtUtc { get; set; }
    public string? Notes { get; set; }
}

