namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;

public class GetDetailedMealStatisticsResponse
{
    public List<DetailedMealItemDto> Data { get; set; } = new();
}

public class DetailedMealItemDto
{
    public DateOnly Date { get; set; }
    public string MealSlot { get; set; } = string.Empty;
    public int? OrganizationId { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public int? MenuId { get; set; }
    public string MenuName { get; set; } = string.Empty;
    public int DishId { get; set; }
    public string DishName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }
}
