using SmartLunch.Backend.Service.Application.OrganizationMealOrders;

namespace SmartLunch.Backend.Service.Application.OrganizationMealContractOrders;

/// <summary>Nháp HĐ đặt suất theo kỳ (Redis).</summary>
public sealed class OrganizationMealPeriodContractDraftPayload
{
    public int UserId { get; set; }
    public int OrganizationId { get; set; }
    public int ContractId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public List<DateOnly> ExcludedDates { get; set; } = new();
    public Dictionary<DateOnly, int> DailyMealOverrides { get; set; } = new();
    public int MealsPerDay { get; set; }
    public int TotalMeals { get; set; }
    public decimal MealUnitPrice { get; set; }
    public int ServiceDays { get; set; }
    public decimal? SubtotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? PromotionCode { get; set; }
    public int? AppliedPromotionId { get; set; }
    public string? AppliedPromotionName { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public OrganizationMealOrderDraftDelivery? Delivery { get; set; }
}
