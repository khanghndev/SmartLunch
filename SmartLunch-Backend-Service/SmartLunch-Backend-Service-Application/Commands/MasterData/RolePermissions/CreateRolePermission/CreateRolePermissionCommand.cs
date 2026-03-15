using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.RolePermissions;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.RolePermissions;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.RolePermissions.CreateRolePermission;

public class CreateRolePermissionCommand : IRequest<CreateRolePermissionResponse>
{
    public CreateRolePermissionRequest Request { get; set; }

    public CreateRolePermissionCommand(CreateRolePermissionRequest request)
    {
        Request = request;
    }
}
