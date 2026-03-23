namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Dishes;

public class GetDishResponse
{
    public DishDto Dish { get; set; } = new();
    public List<DishIngredientQuotaDto> IngredientQuotas { get; set; } = new();
}
