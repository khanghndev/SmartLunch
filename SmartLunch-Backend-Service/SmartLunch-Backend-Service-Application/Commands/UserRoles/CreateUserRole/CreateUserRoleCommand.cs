using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.UserRoles;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserRoles;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.UserRoles.CreateUserRole;

public class CreateUserRoleCommand : IRequest<CreateUserRoleResponse>
{
    public CreateUserRoleRequest Request { get; set; }

    public CreateUserRoleCommand(CreateUserRoleRequest request)
    {
        Request = request;
    }
}
