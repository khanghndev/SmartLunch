namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.DishIngredients;

public class DishIngredientDto
{
    public Guid Id { get; set; }
    public Guid DishId { get; set; }
    public Guid IngredientId { get; set; }
    public string? DishName { get; set; }
    public string? IngredientName { get; set; }
    public decimal Quantity { get; set; }
    public string? Unit { get; set; }
}
