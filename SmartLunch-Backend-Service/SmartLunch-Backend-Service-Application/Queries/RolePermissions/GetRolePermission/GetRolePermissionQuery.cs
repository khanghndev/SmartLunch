using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.RolePermissions;

namespace SmartLunch.Backend.Service.Application.Queries.RolePermissions.GetRolePermission;

public class GetRolePermissionQuery : IRequest<GetRolePermissionResponse>
{
    public int Id { get; set; }

    public GetRolePermissionQuery(int id)
    {
        Id = id;
    }
}
