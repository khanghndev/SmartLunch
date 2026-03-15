using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.UserUnits;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserUnits;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.UserUnits.UpdateUserUnit;

public class UpdateUserUnitCommand : IRequest<GetUserUnitResponse>
{
    public UpdateUserUnitRequest Request { get; set; }

    public UpdateUserUnitCommand(UpdateUserUnitRequest request)
    {
        Request = request;
    }
}
