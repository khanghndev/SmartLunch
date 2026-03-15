using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.UserUnits;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserUnits;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.UserUnits.CreateUserUnit;

public class CreateUserUnitCommand : IRequest<CreateUserUnitResponse>
{
    public CreateUserUnitRequest Request { get; set; }

    public CreateUserUnitCommand(CreateUserUnitRequest request)
    {
        Request = request;
    }
}
