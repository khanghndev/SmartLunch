namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Deliveries;

public class DeliveryDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int? AssignedStaffId { get; set; }
    public string DeliveryAddress { get; set; } = string.Empty;
    public string DeliveryStatus { get; set; } = string.Empty;
    public DateTime? DeliveredAt { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}
