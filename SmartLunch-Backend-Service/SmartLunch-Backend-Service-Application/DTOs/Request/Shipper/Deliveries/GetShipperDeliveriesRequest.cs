namespace SmartLunch.Backend.Service.Application.DTOs.Request.Shipper.Deliveries;

public class GetShipperDeliveriesRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Filter by delivery status (pending/assigned/received/in_transit/completed/failed/rejected).
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Filter by scheduled date of the order (UTC date).
    /// </summary>
    public DateOnly? ScheduledOn { get; set; }
}

