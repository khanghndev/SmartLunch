using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Ingredients;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Ingredients;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Ingredients.UpdateIngredient;

public class UpdateIngredientCommand : IRequest<GetIngredientResponse>
{
    public int IngredientId { get; }
    public UpdateIngredientRequest Request { get; }

    public UpdateIngredientCommand(int ingredientId, UpdateIngredientRequest request)
    {
        IngredientId = ingredientId;
        Request = request;
    }
}
