namespace SmartLunch.Backend.Service.Application.DTOs.Request.OrganizationMealOrders;

/// <summary>
/// Đặt suất đơn vị: giá/suất do FE gửi; mealPlan theo slot main | side | soup (JSON camelCase: main, side, soup).
/// Tổng tiền hợp đồng = Price × (tổng quantity tất cả dòng main trên mọi ngày).
/// </summary>
public sealed class PrepareOrganizationMealContractRequest
{
    public int OrganizationId { get; set; }

    /// <summary>Giá một suất (VND) thỏa thuận — không dùng Dish.Price.</summary>
    public decimal Price { get; set; }

    public List<OrganizationMealDayRequest> MealDays { get; set; } = new();

    /// <summary>Mã khuyến mãi (tùy chọn).</summary>
    public string? PromotionCode { get; set; }

    /// <summary>Id khuyến mãi chọn từ danh sách (ưu tiên hơn mã).</summary>
    public int? PromotionId { get; set; }

    /// <summary>Thông tin giao hàng & người nhận (bắt buộc).</summary>
    public OrganizationMealDeliveryRequest Delivery { get; set; } = new();
}

public sealed class OrganizationMealDayRequest
{
    public DateOnly ServiceDate { get; set; }

    public OrganizationMealPlanSlotsRequest MealPlan { get; set; } = new();
}

public sealed class OrganizationMealPlanSlotsRequest
{
    public List<OrganizationMealLineRequest> Main { get; set; } = new();
    public List<OrganizationMealLineRequest> Side { get; set; } = new();
    public List<OrganizationMealLineRequest> Soup { get; set; } = new();
}

public sealed class OrganizationMealLineRequest
{
    public int DishId { get; set; }
    public int Quantity { get; set; } = 1;
}
