namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.DishIngredients;

public class CreateDishIngredientRequest
{
    public int DishId { get; set; }
    public int IngredientId { get; set; }
    public decimal Quantity { get; set; }
    public string? Unit { get; set; }
}
