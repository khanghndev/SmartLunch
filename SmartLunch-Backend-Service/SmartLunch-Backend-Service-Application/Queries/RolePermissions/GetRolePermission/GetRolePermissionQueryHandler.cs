using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.RolePermissions;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.RolePermissions.GetRolePermission;

public class GetRolePermissionQueryHandler : IRequestHandler<GetRolePermissionQuery, GetRolePermissionResponse>
{
    private readonly IRolePermissionRepository _rolePermissionRepository;

    public GetRolePermissionQueryHandler(IRolePermissionRepository rolePermissionRepository)
    {
        _rolePermissionRepository = rolePermissionRepository;
    }

    public async Task<GetRolePermissionResponse> Handle(GetRolePermissionQuery request, CancellationToken cancellationToken)
    {
        var rp = await _rolePermissionRepository.GetByIdAsync(request.Id);
        if (rp == null)
            return new GetRolePermissionResponse { RolePermission = new RolePermissionDto() };

        return new GetRolePermissionResponse
        {
            RolePermission = new RolePermissionDto
            {
                Id = rp.Id,
                RoleId = rp.RoleId,
                PermissionId = rp.PermissionId,
                RoleName = rp.Role?.Name,
                PermissionName = rp.Permission?.Name,
                AssignedAt = rp.AssignedAt,
                AssignedBy = rp.AssignedBy,
                IsActive = rp.IsActive
            }
        };
    }
}
