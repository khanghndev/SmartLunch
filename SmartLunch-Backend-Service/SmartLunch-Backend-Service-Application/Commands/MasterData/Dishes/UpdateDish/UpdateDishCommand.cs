using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Dishes;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Dishes;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Dishes.UpdateDish;

public class UpdateDishCommand : IRequest<GetDishResponse>
{
    public Guid DishId { get; }
    public UpdateDishRequest Request { get; }

    public UpdateDishCommand(Guid dishId, UpdateDishRequest request)
    {
        DishId = dishId;
        Request = request;
    }
}
