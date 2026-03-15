using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.DishIngredients;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.DishIngredients;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.DishIngredients.CreateDishIngredient;

public class CreateDishIngredientCommand : IRequest<CreateDishIngredientResponse>
{
    public CreateDishIngredientRequest Request { get; set; }

    public CreateDishIngredientCommand(CreateDishIngredientRequest request)
    {
        Request = request;
    }
}
