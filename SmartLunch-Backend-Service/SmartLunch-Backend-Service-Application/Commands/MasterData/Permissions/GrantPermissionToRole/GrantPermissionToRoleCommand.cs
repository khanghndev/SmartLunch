using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Permissions;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Permissions;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Permissions.GrantPermissionToRole;

public class GrantPermissionToRoleCommand : IRequest<GrantPermissionToRoleResponse>
{
    public GrantPermissionToRoleRequest Request { get; set; }

    public GrantPermissionToRoleCommand(GrantPermissionToRoleRequest request)
    {
        Request = request;
    }
}
