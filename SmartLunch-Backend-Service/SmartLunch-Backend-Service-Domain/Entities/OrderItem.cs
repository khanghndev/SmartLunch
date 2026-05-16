namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Order line item
/// </summary>
public class OrderItem
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public int OrderId { get; set; }
    public int DishId { get; set; }
    public int Quantity { get; set; } = 1;
    /// <summary>Ngày giao suất (đơn đặt nhiều ngày); null với đơn một ngày / giỏ hàng.</summary>
    public DateOnly? ServiceDate { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }

    public virtual Order Order { get; set; } = null!;
    public virtual Dish Dish { get; set; } = null!;
}
