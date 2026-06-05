namespace SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealContractOrders;

public sealed class SubmitOrganizationMealWeeklySelectionResponse
{
    public int ContractId { get; set; }
    public int WeeklySelectionId { get; set; }
    public int OrderId { get; set; }
    public DateOnly WeekStart { get; set; }
    public string Status { get; set; } = string.Empty;
    public int ItemCount { get; set; }
}
