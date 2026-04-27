using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.DishIngredients;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.DishIngredients;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.DishIngredients.UpdateDishIngredient;

public class UpdateDishIngredientCommand : IRequest<GetDishIngredientResponse>
{
    public UpdateDishIngredientRequest Request { get; set; }

    public UpdateDishIngredientCommand(UpdateDishIngredientRequest request)
    {
        Request = request;
    }
}
