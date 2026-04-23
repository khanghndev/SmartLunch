namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;

public class GetMealStatisticsResponse
{
    public List<MealStatisticItemDto> Data { get; set; } = new();
}

public class MealStatisticItemDto
{
    public DateOnly Date { get; set; }
    public string MealSlot { get; set; } = string.Empty;
    public Guid? UnitId { get; set; }
    public string UnitName { get; set; } = string.Empty;
    public int TotalMeals { get; set; }
    public decimal TotalAmount { get; set; }
}
