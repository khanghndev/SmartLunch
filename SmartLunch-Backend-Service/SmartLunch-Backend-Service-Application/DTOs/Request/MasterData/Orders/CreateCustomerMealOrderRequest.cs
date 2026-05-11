namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Orders;

/// <summary>Đặt món trực tuyến (B2C): một ngày giao, nhiều dòng món.</summary>
public class CreateCustomerMealOrderRequest
{
    /// <summary>Ngày ăn / giao (theo lịch đơn).</summary>
    public DateOnly ScheduledDate { get; set; }

    public List<CreateCustomerMealOrderLineRequest> Lines { get; set; } = new();
}

public class CreateCustomerMealOrderLineRequest
{
    public int DishId { get; set; }
    public int Quantity { get; set; } = 1;
}
