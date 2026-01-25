using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Permissions;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Permissions;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Permissions.RevokePermissionFromRole;

public class RevokePermissionFromRoleCommand : IRequest<RevokePermissionFromRoleResponse>
{
    public RevokePermissionFromRoleRequest Request { get; set; }

    public RevokePermissionFromRoleCommand(RevokePermissionFromRoleRequest request)
    {
        Request = request;
    }
}
