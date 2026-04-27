using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.RolePermissions;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.RolePermissions;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.RolePermissions.UpdateRolePermission;

public class UpdateRolePermissionCommandHandler : IRequestHandler<UpdateRolePermissionCommand, GetRolePermissionResponse>
{
    private readonly IRolePermissionRepository _rolePermissionRepository;

    public UpdateRolePermissionCommandHandler(IRolePermissionRepository rolePermissionRepository)
    {
        _rolePermissionRepository = rolePermissionRepository;
    }

    public async Task<GetRolePermissionResponse> Handle(UpdateRolePermissionCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;
        var entity = await _rolePermissionRepository.GetByIdAsync(req.Id);
        if (entity == null)
            throw new KeyNotFoundException($"RolePermission with ID {req.Id} not found");

        entity.IsActive = req.IsActive;
        await _rolePermissionRepository.UpdateAsync(entity);

        return new GetRolePermissionResponse
        {
            RolePermission = new RolePermissionDto
            {
                Id = entity.Id,
                RoleId = entity.RoleId,
                PermissionId = entity.PermissionId,
                RoleName = entity.Role?.Name,
                PermissionName = entity.Permission?.Name,
                AssignedAt = entity.AssignedAt,
                AssignedBy = entity.AssignedBy,
                IsActive = entity.IsActive
            }
        };
    }
}
