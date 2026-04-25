using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Roles;

namespace SmartLunch.Backend.Service.Application.Queries.Roles.GetRole;

/// <summary>
/// Query to get a role by ID
/// </summary>
public class GetRoleQuery : IRequest<GetRoleResponse>
{
    public int RoleId { get; set; }

    public GetRoleQuery(int roleId)
    {
        RoleId = roleId;
    }
}
