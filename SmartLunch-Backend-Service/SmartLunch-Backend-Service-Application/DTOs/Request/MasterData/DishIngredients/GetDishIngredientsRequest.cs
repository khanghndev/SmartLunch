namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.DishIngredients;

public class GetDishIngredientsRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public Guid? DishId { get; set; }
    public Guid? IngredientId { get; set; }
}
