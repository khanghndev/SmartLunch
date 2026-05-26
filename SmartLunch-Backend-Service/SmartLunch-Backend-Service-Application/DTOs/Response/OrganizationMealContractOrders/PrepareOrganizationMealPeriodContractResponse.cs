using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealOrders;

namespace SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealContractOrders;

public sealed class PrepareOrganizationMealPeriodContractResponse
{
    public string DraftId { get; set; } = string.Empty;
    public int ContractId { get; set; }
    public string? ContractNumber { get; set; }
    public string? ContractFileUrl { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public List<DateOnly> ExcludedDates { get; set; } = new();
    public int ServiceDays { get; set; }
    public int MealsPerDay { get; set; }
    public decimal MealUnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public OrganizationMealDeliverySummaryDto? Delivery { get; set; }
}
