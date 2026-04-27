using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Roles;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Roles;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Roles.RevokeRoleFromUser;

public class RevokeRoleFromUserCommand : IRequest<RevokeRoleFromUserResponse>
{
    public RevokeRoleFromUserRequest Request { get; set; }

    public RevokeRoleFromUserCommand(RevokeRoleFromUserRequest request)
    {
        Request = request;
    }
}
