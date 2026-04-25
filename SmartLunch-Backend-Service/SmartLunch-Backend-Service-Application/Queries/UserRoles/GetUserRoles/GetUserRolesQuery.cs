using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserRoles;

namespace SmartLunch.Backend.Service.Application.Queries.UserRoles.GetUserRoles;

public class GetUserRolesQuery : IRequest<GetUserRolesResponse>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int? UserId { get; set; }
    public int? RoleId { get; set; }
    public bool? IsActive { get; set; }

    public GetUserRolesQuery(int page, int pageSize, int? userId, int? roleId, bool? isActive)
    {
        Page = page;
        PageSize = pageSize;
        UserId = userId;
        RoleId = roleId;
        IsActive = isActive;
    }
}
