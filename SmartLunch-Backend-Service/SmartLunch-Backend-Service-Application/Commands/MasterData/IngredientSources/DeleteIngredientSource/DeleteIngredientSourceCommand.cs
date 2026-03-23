using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.IngredientSources;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.IngredientSources.DeleteIngredientSource;

public class DeleteIngredientSourceCommand : IRequest<DeleteIngredientSourceResponse>
{
    public Guid IngredientSourceId { get; }

    public DeleteIngredientSourceCommand(Guid ingredientSourceId)
    {
        IngredientSourceId = ingredientSourceId;
    }
}
