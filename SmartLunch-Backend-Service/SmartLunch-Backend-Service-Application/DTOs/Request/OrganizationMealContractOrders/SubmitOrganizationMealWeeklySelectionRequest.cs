using SmartLunch.Backend.Service.Application.DTOs.Request.OrganizationMealOrders;

namespace SmartLunch.Backend.Service.Application.DTOs.Request.OrganizationMealContractOrders;

/// <summary>Chọn món tuần — chỉ món chính (mealPlan.main).</summary>
public sealed class SubmitOrganizationMealWeeklySelectionRequest
{
    /// <summary>Thứ 2 của tuần cần đặt món.</summary>
    public DateOnly WeekStart { get; set; }

    /// <summary>Mỗi ngày: mealPlan.main (bắt buộc). Không dùng side/soup.</summary>
    public List<OrganizationMealDayRequest> MealDays { get; set; } = new();
}
