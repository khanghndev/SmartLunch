namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Deliveries;

public class DeliveryDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid? AssignedStaffId { get; set; }
    public string DeliveryAddress { get; set; } = string.Empty;
    public string DeliveryStatus { get; set; } = string.Empty;
    public DateTime? DeliveredAt { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}
