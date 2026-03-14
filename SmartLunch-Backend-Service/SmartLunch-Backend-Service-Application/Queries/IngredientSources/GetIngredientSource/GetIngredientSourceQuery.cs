using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.IngredientSources;

namespace SmartLunch.Backend.Service.Application.Queries.IngredientSources.GetIngredientSource;

public class GetIngredientSourceQuery : IRequest<GetIngredientSourceResponse>
{
    public Guid IngredientSourceId { get; set; }

    public GetIngredientSourceQuery(Guid ingredientSourceId)
    {
        IngredientSourceId = ingredientSourceId;
    }
}
