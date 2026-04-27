using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Dishes;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Dishes;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Dishes.CreateDish;

public class CreateDishCommand : IRequest<GetDishResponse>
{
    public CreateDishRequest Request { get; }

    public CreateDishCommand(CreateDishRequest request)
    {
        Request = request;
    }
}
