namespace SmartLunch.Backend.Service.Application.DTOs.Request.Cart;

public class CheckoutCartRequest
{
    /// <summary>
    /// Các ngày trên menu mà khách không muốn nhận/ăn — bỏ qua những ngày này khi gom món từ thực đơn trong giỏ.
    /// Ngày so khớp theo phần lịch (calendar day), cùng cách nhìn như ngày trên MenuSchedule.
    /// </summary>
    public List<DateOnly> ExcludedDates { get; set; } = new();
}
