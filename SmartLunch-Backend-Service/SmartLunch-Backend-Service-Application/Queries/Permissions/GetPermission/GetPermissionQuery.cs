using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Permissions;

namespace SmartLunch.Backend.Service.Application.Queries.Permissions.GetPermission;

/// <summary>
/// Query to get a permission by ID
/// </summary>
public class GetPermissionQuery : IRequest<GetPermissionResponse>
{
    public Guid PermissionId { get; set; }

    public GetPermissionQuery(Guid permissionId)
    {
        PermissionId = permissionId;
    }
}
