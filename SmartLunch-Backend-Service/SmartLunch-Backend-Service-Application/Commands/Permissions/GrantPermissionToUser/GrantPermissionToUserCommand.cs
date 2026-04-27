using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Permissions;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Permissions;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Permissions.GrantPermissionToUser;

public class GrantPermissionToUserCommand : IRequest<GrantPermissionToUserResponse>
{
    public GrantPermissionToUserRequest Request { get; set; }

    public GrantPermissionToUserCommand(GrantPermissionToUserRequest request)
    {
        Request = request;
    }
}
