namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Delivery record
/// </summary>
public class Delivery
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public int OrderId { get; set; }
    public int? AssignedStaffId { get; set; }
    public string DeliveryAddress { get; set; } = string.Empty;
    public string DeliveryStatus { get; set; } = "pending"; // pending|in_transit|completed|failed
    public DateTime? DeliveredAt { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual Order Order { get; set; } = null!;
    public virtual User? AssignedStaff { get; set; }
}
