namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Contracts;

public sealed class ContractDailyMealPortionDto
{
    public DateOnly ServiceDate { get; set; }
    public int MealCount { get; set; }
}
