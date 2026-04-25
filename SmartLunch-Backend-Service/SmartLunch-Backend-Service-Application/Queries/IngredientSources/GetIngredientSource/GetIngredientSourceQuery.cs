using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.IngredientSources;

namespace SmartLunch.Backend.Service.Application.Queries.IngredientSources.GetIngredientSource;

public class GetIngredientSourceQuery : IRequest<GetIngredientSourceResponse>
{
    public int IngredientSourceId { get; set; }

    public GetIngredientSourceQuery(int ingredientSourceId)
    {
        IngredientSourceId = ingredientSourceId;
    }
}
