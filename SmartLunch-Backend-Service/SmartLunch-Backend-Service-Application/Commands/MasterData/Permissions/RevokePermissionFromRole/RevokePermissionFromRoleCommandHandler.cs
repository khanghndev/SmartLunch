using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Permissions;
using SmartLunch.Backend.Service.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Permissions.RevokePermissionFromRole;

public class RevokePermissionFromRoleCommandHandler : IRequestHandler<RevokePermissionFromRoleCommand, RevokePermissionFromRoleResponse>
{
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly ILogger<RevokePermissionFromRoleCommandHandler> _logger;

    public RevokePermissionFromRoleCommandHandler(
        IRolePermissionRepository rolePermissionRepository,
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        ILogger<RevokePermissionFromRoleCommandHandler> logger)
    {
        _rolePermissionRepository = rolePermissionRepository;
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _logger = logger;
    }

    public async Task<RevokePermissionFromRoleResponse> Handle(RevokePermissionFromRoleCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;

        // Validate role exists
        var role = await _roleRepository.GetByIdAsync(req.RoleId);
        if (role == null)
        {
            throw new KeyNotFoundException($"Role with ID {req.RoleId} not found");
        }

        // Validate permission exists
        var permission = await _permissionRepository.GetByIdAsync(req.PermissionId);
        if (permission == null)
        {
            throw new KeyNotFoundException($"Permission with ID {req.PermissionId} not found");
        }

        // Check if role has this permission
        var rolePermission = await _rolePermissionRepository.GetByRoleAndPermissionAsync(req.RoleId, req.PermissionId);
        if (rolePermission == null || !rolePermission.IsActive)
        {
            throw new InvalidOperationException($"Role does not have active permission {permission.Name}");
        }

        // Deactivate the permission assignment (soft delete)
        rolePermission.IsActive = false;
        await _rolePermissionRepository.UpdateAsync(rolePermission);

        _logger.LogInformation("Revoked permission {PermissionName} from role {RoleName}", permission.Name, role.Name);

        return new RevokePermissionFromRoleResponse
        {
            RoleId = req.RoleId,
            PermissionId = req.PermissionId,
            Message = $"Permission {permission.Name} revoked from role {role.Name} successfully"
        };
    }
}
