using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Permissions;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Permissions;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Permissions.RevokePermissionFromUser;

public class RevokePermissionFromUserCommand : IRequest<RevokePermissionFromUserResponse>
{
    public RevokePermissionFromUserRequest Request { get; set; }

    public RevokePermissionFromUserCommand(RevokePermissionFromUserRequest request)
    {
        Request = request;
    }
}
