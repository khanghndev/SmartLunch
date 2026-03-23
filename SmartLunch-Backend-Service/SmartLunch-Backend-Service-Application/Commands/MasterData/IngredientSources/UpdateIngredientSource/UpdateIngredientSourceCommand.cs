using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.IngredientSources;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.IngredientSources;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.IngredientSources.UpdateIngredientSource;

public class UpdateIngredientSourceCommand : IRequest<GetIngredientSourceResponse>
{
    public Guid IngredientSourceId { get; }
    public UpdateIngredientSourceRequest Request { get; }

    public UpdateIngredientSourceCommand(Guid ingredientSourceId, UpdateIngredientSourceRequest request)
    {
        IngredientSourceId = ingredientSourceId;
        Request = request;
    }
}
