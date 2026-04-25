using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.DishIngredients;

namespace SmartLunch.Backend.Service.Application.Queries.DishIngredients.GetDishIngredients;

public class GetDishIngredientsQuery : IRequest<GetDishIngredientsResponse>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int? DishId { get; set; }
    public int? IngredientId { get; set; }

    public GetDishIngredientsQuery(int page, int pageSize, int? dishId, int? ingredientId)
    {
        Page = page;
        PageSize = pageSize;
        DishId = dishId;
        IngredientId = ingredientId;
    }
}
