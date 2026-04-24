namespace SmartLunch.Backend.Service.Application.DTOs.Request.Cart;

public class CheckoutCartRequest
{
    /// <summary>Ngày nhận/ăn. Nếu bỏ trống thì dùng ngày hiện tại (UTC).</summary>
    public DateOnly? ScheduledDate { get; set; }
}
