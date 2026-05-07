namespace SmartLunch.Backend.Service.Application.DTOs.Request.Shipper.Deliveries;

public class UpdateShipperDeliveryStatusRequest
{
    /// <summary>
    /// received | in_transit | completed | failed | rejected
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Required when status = failed or rejected.
    /// </summary>
    public string? Notes { get; set; }
}

