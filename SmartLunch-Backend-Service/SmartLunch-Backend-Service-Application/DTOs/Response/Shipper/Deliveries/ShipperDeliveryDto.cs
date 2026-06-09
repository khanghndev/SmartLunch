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
    public string? RecipientConfirmedName { get; set; }
    public DateTime? RecipientConfirmedAtUtc { get; set; }
    public string? RecipientSignatureUrl { get; set; }
    public string? ShipperSignatureUrl { get; set; }
    /// <summary>PDF biên bản bàn giao có chữ ký người nhận.</summary>
    public string? HandoverDocumentUrl { get; set; }
    /// <summary>Đơn đang giao — cần chữ ký người nhận khi POST /proof.</summary>
    public bool RequiresRecipientSignature { get; set; }
}

