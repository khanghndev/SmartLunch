namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.DishIngredients;

public class CreateDishIngredientRequest
{
    public Guid DishId { get; set; }
    public Guid IngredientId { get; set; }
    public decimal Quantity { get; set; }
    public string? Unit { get; set; }
}
