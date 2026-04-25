namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;

public class OrderItemLineDto
{
    public int Id { get; set; }
    public int DishId { get; set; }
    public string DishName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
