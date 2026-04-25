namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.DishIngredients;

public class GetDishIngredientsRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int? DishId { get; set; }
    public int? IngredientId { get; set; }
}
