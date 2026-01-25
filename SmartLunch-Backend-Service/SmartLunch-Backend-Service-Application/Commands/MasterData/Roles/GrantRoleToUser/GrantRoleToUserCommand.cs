using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Roles;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Roles;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Roles.GrantRoleToUser;

public class GrantRoleToUserCommand : IRequest<GrantRoleToUserResponse>
{
    public GrantRoleToUserRequest Request { get; set; }

    public GrantRoleToUserCommand(GrantRoleToUserRequest request)
    {
        Request = request;
    }
}
