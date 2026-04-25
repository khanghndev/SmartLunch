namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.DishIngredients;

public class CreateDishIngredientResponse
{
    public int Id { get; set; }
    public int DishId { get; set; }
    public int IngredientId { get; set; }
    public string Message { get; set; } = "Dish ingredient created successfully";
}
