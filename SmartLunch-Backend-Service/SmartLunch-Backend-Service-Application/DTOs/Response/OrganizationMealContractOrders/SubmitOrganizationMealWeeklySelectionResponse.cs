namespace SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealContractOrders;

public sealed class SubmitOrganizationMealWeeklySelectionResponse
{
    public int ContractId { get; set; }
    public int OrderId { get; set; }
    public DateOnly WeekStart { get; set; }
    public int ItemCount { get; set; }
}
