namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.DishIngredients;

public class UpdateDishIngredientRequest
{
    public Guid Id { get; set; }
    public decimal Quantity { get; set; }
    public string? Unit { get; set; }
}
