namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Order line item
/// </summary>
public class OrderItem
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid DishId { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }

    public virtual Order Order { get; set; } = null!;
    public virtual Dish Dish { get; set; } = null!;
}
