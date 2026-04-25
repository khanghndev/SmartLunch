namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;

public class OrderDto
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public int? UnitId { get; set; }
    public string? UnitName { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? InvoiceCode { get; set; }
    public int? CreatedBySalesUserId { get; set; }
    public string? CreatedBySalesDisplayName { get; set; }
    public List<OrderItemLineDto> Items { get; set; } = new();
}
