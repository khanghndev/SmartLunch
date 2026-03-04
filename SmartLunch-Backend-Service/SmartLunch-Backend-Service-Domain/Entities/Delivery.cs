namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Delivery record
/// </summary>
public class Delivery
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid? AssignedStaffId { get; set; }
    public string DeliveryAddress { get; set; } = string.Empty;
    public string DeliveryStatus { get; set; } = "pending"; // pending|in_transit|completed|failed
    public DateTime? DeliveredAt { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual Order Order { get; set; } = null!;
    public virtual User? AssignedStaff { get; set; }
}
