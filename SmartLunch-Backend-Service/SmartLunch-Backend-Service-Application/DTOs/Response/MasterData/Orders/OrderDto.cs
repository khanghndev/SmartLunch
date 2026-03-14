namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;

public class OrderDto
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public Guid? UnitId { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
