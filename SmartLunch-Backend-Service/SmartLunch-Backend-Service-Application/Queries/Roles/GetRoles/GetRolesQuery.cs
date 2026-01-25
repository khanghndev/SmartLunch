using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Roles;

namespace SmartLunch.Backend.Service.Application.Queries.Roles.GetRoles;
/// <summary>
/// Query to get list of roles
/// </summary>
public class GetRolesQuery : IRequest<GetRolesResponse>
{
    public bool? IsActive { get; set; }

    public GetRolesQuery(bool? isActive = null)
    {
        IsActive = isActive;
    }
}
