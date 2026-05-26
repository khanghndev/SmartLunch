namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Dishes;

/// <summary>Mức giá suất ăn (dish_values).</summary>
public class DishValueDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string? Label { get; set; }
    public int SortOrder { get; set; }
}
