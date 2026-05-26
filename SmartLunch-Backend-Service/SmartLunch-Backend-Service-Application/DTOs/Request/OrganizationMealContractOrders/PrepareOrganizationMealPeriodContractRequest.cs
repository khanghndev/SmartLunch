using SmartLunch.Backend.Service.Application.DTOs.Request.OrganizationMealOrders;

namespace SmartLunch.Backend.Service.Application.DTOs.Request.OrganizationMealContractOrders;

public sealed class PrepareOrganizationMealPeriodContractRequest
{
    public int OrganizationId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    /// <summary>Ngày không cung cấp suất trong thời hạn HĐ.</summary>
    public List<DateOnly> ExcludedDates { get; set; } = new();

    /// <summary>Số suất ăn mỗi ngày phục vụ.</summary>
    public int MealsPerDay { get; set; }

    /// <summary>Giá một suất (VND).</summary>
    public decimal MealUnitPrice { get; set; }

    public OrganizationMealDeliveryRequest Delivery { get; set; } = new();
}
