using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserPermissions;

namespace SmartLunch.Backend.Service.Application.Queries.UserPermissions.GetUserPermissions;

public class GetUserPermissionsQuery : IRequest<GetUserPermissionsResponse>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int? UserId { get; set; }
    public int? PermissionId { get; set; }
    public bool? IsActive { get; set; }

    public GetUserPermissionsQuery(int page, int pageSize, int? userId, int? permissionId, bool? isActive)
    {
        Page = page;
        PageSize = pageSize;
        UserId = userId;
        PermissionId = permissionId;
        IsActive = isActive;
    }
}
