namespace SmartLunch.Backend.Service.Application.DTOs.Request.Customer.Reviews;

public class CreateCustomerReviewRequest
{
    /// <summary>Đánh giá cho món ăn (chọn 1 trong 2: DishId hoặc OrderId).</summary>
    public int? DishId { get; set; }

    /// <summary>Đánh giá cho đơn hàng (chọn 1 trong 2: DishId hoặc OrderId).</summary>
    public int? OrderId { get; set; }

    /// <summary>1–5 sao.</summary>
    public int Rating { get; set; }

    public string? Comment { get; set; }
}

