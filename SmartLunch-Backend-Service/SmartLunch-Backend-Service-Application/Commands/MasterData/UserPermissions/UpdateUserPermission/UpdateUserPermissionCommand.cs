using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.UserPermissions;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserPermissions;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.UserPermissions.UpdateUserPermission;

public class UpdateUserPermissionCommand : IRequest<GetUserPermissionResponse>
{
    public UpdateUserPermissionRequest Request { get; set; }

    public UpdateUserPermissionCommand(UpdateUserPermissionRequest request)
    {
        Request = request;
    }
}
