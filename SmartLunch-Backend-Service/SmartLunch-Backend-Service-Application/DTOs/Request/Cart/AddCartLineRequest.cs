namespace SmartLunch.Backend.Service.Application.DTOs.Request.Cart;

public class AddCartLineRequest
{
    public Guid DishId { get; set; }
    public int Quantity { get; set; } = 1;

    /// <summary>Đơn vị đặt (tùy chọn).</summary>
    public Guid? UnitId { get; set; }
}
