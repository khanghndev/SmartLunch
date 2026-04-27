using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.RolePermissions;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.RolePermissions;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.RolePermissions.UpdateRolePermission;

public class UpdateRolePermissionCommand : IRequest<GetRolePermissionResponse>
{
    public UpdateRolePermissionRequest Request { get; set; }

    public UpdateRolePermissionCommand(UpdateRolePermissionRequest request)
    {
        Request = request;
    }
}
