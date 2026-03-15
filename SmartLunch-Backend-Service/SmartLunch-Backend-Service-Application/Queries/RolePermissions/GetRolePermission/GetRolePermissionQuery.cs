using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.RolePermissions;

namespace SmartLunch.Backend.Service.Application.Queries.RolePermissions.GetRolePermission;

public class GetRolePermissionQuery : IRequest<GetRolePermissionResponse>
{
    public Guid Id { get; set; }

    public GetRolePermissionQuery(Guid id)
    {
        Id = id;
    }
}
