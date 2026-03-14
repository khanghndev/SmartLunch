namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.OrderItems;

public class OrderItemDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid DishId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
