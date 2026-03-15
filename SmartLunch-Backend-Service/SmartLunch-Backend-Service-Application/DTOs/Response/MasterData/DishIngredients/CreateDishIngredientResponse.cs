namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.DishIngredients;

public class CreateDishIngredientResponse
{
    public Guid Id { get; set; }
    public Guid DishId { get; set; }
    public Guid IngredientId { get; set; }
    public string Message { get; set; } = "Dish ingredient created successfully";
}
