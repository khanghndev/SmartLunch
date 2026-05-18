using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Ingredients;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Ingredients;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Ingredients.CreateIngredient;

public class CreateIngredientCommand : IRequest<GetIngredientResponse>
{
    public CreateIngredientRequest Request { get; }

    public CreateIngredientCommand(CreateIngredientRequest request)
    {
        Request = request;
    }
}
