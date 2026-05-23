namespace SmartLunch.Backend.Service.Application.DTOs.Request.Manager.Deliveries;

public sealed class GetManagerDeliveriesRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? Status { get; set; }
    public DateOnly? ScheduledOn { get; set; }
    public string? SearchTerm { get; set; }
    public bool? UnassignedOnly { get; set; }
}

public sealed class AssignManagerDeliveryRequest
{
    public int ShipperUserId { get; set; }
    public string? Notes { get; set; }
}
