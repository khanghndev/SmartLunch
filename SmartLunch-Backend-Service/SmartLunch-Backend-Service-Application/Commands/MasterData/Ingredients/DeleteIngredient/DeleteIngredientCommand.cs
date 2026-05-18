using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Ingredients;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Ingredients.DeleteIngredient;

public class DeleteIngredientCommand : IRequest<DeleteIngredientResponse>
{
    public int IngredientId { get; }

    public DeleteIngredientCommand(int ingredientId)
    {
        IngredientId = ingredientId;
    }
}
