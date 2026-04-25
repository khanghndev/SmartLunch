namespace SmartLunch.Backend.Service.Application.DTOs.Response.Cart;

public class ShoppingCartDto
{
    public int UserId { get; set; }
    public int? UnitId { get; set; }
    public List<ShoppingCartLineDto> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}

public class ShoppingCartLineDto
{
    public int Id { get; set; }
    public int DishId { get; set; }
    public string DishName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
}
