using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Ingredients;

namespace SmartLunch.Backend.Service.Application.Queries.Ingredients.GetIngredient;

public class GetIngredientQuery : IRequest<GetIngredientResponse>
{
    public int IngredientId { get; set; }

    public GetIngredientQuery(int ingredientId)
    {
        IngredientId = ingredientId;
    }
}
