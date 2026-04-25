using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Dishes;

namespace SmartLunch.Backend.Service.Application.Queries.Dishes.GetDish;

/// <summary>
/// Query to get a dish by ID
/// </summary>
public class GetDishQuery : IRequest<GetDishResponse>
{
    public int DishId { get; set; }

    /// <summary>Kèm định mức nguyên liệu (DishIngredient).</summary>
    public bool IncludeIngredientQuotas { get; set; }

    public GetDishQuery(int dishId, bool includeIngredientQuotas = false)
    {
        DishId = dishId;
        IncludeIngredientQuotas = includeIngredientQuotas;
    }
}
