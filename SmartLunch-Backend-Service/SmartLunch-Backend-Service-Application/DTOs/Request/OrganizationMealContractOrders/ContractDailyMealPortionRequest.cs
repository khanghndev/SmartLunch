namespace SmartLunch.Backend.Service.Application.DTOs.Request.OrganizationMealContractOrders;

public sealed class ContractDailyMealPortionRequest
{
    public DateOnly ServiceDate { get; set; }
    public int MealCount { get; set; }
}
