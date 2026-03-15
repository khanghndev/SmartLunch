using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.RolePermissions;

namespace SmartLunch.Backend.Service.Application.Queries.RolePermissions.GetRolePermissions;

public class GetRolePermissionsQuery : IRequest<GetRolePermissionsResponse>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public Guid? RoleId { get; set; }
    public Guid? PermissionId { get; set; }
    public bool? IsActive { get; set; }

    public GetRolePermissionsQuery(int page, int pageSize, Guid? roleId, Guid? permissionId, bool? isActive)
    {
        Page = page;
        PageSize = pageSize;
        RoleId = roleId;
        PermissionId = permissionId;
        IsActive = isActive;
    }
}
