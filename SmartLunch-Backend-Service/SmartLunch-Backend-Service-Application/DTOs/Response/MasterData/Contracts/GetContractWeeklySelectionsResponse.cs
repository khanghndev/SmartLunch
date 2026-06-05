namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Contracts;

using SmartLunch.Backend.Service.Domain.Entities;

public sealed class GetContractWeeklySelectionsResponse
{
    public int ContractId { get; set; }
    public DateOnly? OpenWeekMonday { get; set; }
    public int TotalServiceWeeks { get; set; }
    public int FilledServiceWeeks { get; set; }
    public List<ContractWeeklySelectionDto> Weeks { get; set; } = new();
}

public sealed class ContractWeeklySelectionDto
{
    public int Id { get; set; }
    public DateOnly WeekMonday { get; set; }
    public DateOnly WeekEnd { get; set; }
    public string Status { get; set; } = ContractWeeklySelectionStatuses.Pending;
    public int? FulfillmentOrderId { get; set; }
    public DateTime? SelectedAt { get; set; }
    public bool IsOpenWeek { get; set; }
    public bool CanSelect { get; set; }
    public bool IsFilled { get; set; }
    public List<ContractWeeklySelectionDayDto> Days { get; set; } = new();
}

public sealed class ContractWeeklySelectionDayDto
{
    public DateOnly ServiceDate { get; set; }
    public int RequiredMeals { get; set; }
    public List<ContractWeeklySelectionLineDto> Main { get; set; } = new();
}

public sealed class ContractWeeklySelectionLineDto
{
    public int DishId { get; set; }
    public string DishName { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
