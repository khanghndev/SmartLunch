using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.UserPermissions;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserPermissions;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.UserPermissions.CreateUserPermission;

public class CreateUserPermissionCommand : IRequest<CreateUserPermissionResponse>
{
    public CreateUserPermissionRequest Request { get; set; }

    public CreateUserPermissionCommand(CreateUserPermissionRequest request)
    {
        Request = request;
    }
}
