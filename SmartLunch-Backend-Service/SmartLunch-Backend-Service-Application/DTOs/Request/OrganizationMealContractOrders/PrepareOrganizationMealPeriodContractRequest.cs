using SmartLunch.Backend.Service.Application.DTOs.Request.OrganizationMealOrders;

namespace SmartLunch.Backend.Service.Application.DTOs.Request.OrganizationMealContractOrders;

public sealed class PrepareOrganizationMealPeriodContractRequest
{
    public int OrganizationId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    /// <summary>Ngày không cung cấp suất trong thời hạn HĐ.</summary>
    public List<DateOnly> ExcludedDates { get; set; } = new();

    /// <summary>Số suất ăn mỗi ngày phục vụ (mặc định).</summary>
    public int MealsPerDay { get; set; }

    /// <summary>Số suất tùy chỉnh theo ngày (chỉ gửi ngày khác MealsPerDay).</summary>
    public List<ContractDailyMealPortionRequest> DailyMealPortions { get; set; } = new();

    /// <summary>Giá một suất (VND).</summary>
    public decimal MealUnitPrice { get; set; }

    public string? PromotionCode { get; set; }
    public int? PromotionId { get; set; }

    public OrganizationMealDeliveryRequest Delivery { get; set; } = new();
}
