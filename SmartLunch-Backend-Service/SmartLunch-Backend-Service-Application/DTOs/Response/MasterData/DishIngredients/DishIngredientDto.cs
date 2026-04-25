namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.DishIngredients;

public class DishIngredientDto
{
    public int Id { get; set; }
    public int DishId { get; set; }
    public int IngredientId { get; set; }
    public string? DishName { get; set; }
    public string? IngredientName { get; set; }
    public decimal Quantity { get; set; }
    public string? Unit { get; set; }
}
