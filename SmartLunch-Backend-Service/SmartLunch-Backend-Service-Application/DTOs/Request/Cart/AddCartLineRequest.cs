namespace SmartLunch.Backend.Service.Application.DTOs.Request.Cart;

public class AddCartLineRequest
{
    public int DishId { get; set; }
    public int Quantity { get; set; } = 1;

    /// <summary>Tổ chức đặt (tùy chọn).</summary>
    public int? OrganizationId { get; set; }
}
