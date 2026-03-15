using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.RolePermissions;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.RolePermissions.GetRolePermissions;

public class GetRolePermissionsQueryHandler : IRequestHandler<GetRolePermissionsQuery, GetRolePermissionsResponse>
{
    private readonly IRolePermissionRepository _rolePermissionRepository;

    public GetRolePermissionsQueryHandler(IRolePermissionRepository rolePermissionRepository)
    {
        _rolePermissionRepository = rolePermissionRepository;
    }

    public async Task<GetRolePermissionsResponse> Handle(GetRolePermissionsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _rolePermissionRepository.GetPagedAsync(
            request.Page, request.PageSize, request.RoleId, request.PermissionId, request.IsActive);

        var dtos = items.Select(rp => new RolePermissionDto
        {
            Id = rp.Id,
            RoleId = rp.RoleId,
            PermissionId = rp.PermissionId,
            RoleName = rp.Role?.Name,
            PermissionName = rp.Permission?.Name,
            AssignedAt = rp.AssignedAt,
            AssignedBy = rp.AssignedBy,
            IsActive = rp.IsActive
        }).ToList();

        return new GetRolePermissionsResponse
        {
            Data = dtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
