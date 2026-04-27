using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.UserRoles;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserRoles;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.UserRoles.UpdateUserRole;

public class UpdateUserRoleCommand : IRequest<GetUserRoleResponse>
{
    public UpdateUserRoleRequest Request { get; set; }

    public UpdateUserRoleCommand(UpdateUserRoleRequest request)
    {
        Request = request;
    }
}
