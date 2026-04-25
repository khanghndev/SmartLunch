using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Permissions;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Permissions.GrantPermissionToRole;

public class GrantPermissionToRoleCommandHandler : IRequestHandler<GrantPermissionToRoleCommand, GrantPermissionToRoleResponse>
{
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly ILogger<GrantPermissionToRoleCommandHandler> _logger;

    public GrantPermissionToRoleCommandHandler(
        IRolePermissionRepository rolePermissionRepository,
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        ILogger<GrantPermissionToRoleCommandHandler> logger)
    {
        _rolePermissionRepository = rolePermissionRepository;
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _logger = logger;
    }

    public async Task<GrantPermissionToRoleResponse> Handle(GrantPermissionToRoleCommand request, CancellationToken cancellationToken)
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

        // Check if role is active
        if (!role.IsActive)
        {
            throw new InvalidOperationException($"Role {role.Name} is not active");
        }

        // Check if permission is active
        if (!permission.IsActive)
        {
            throw new InvalidOperationException($"Permission {permission.Name} is not active");
        }

        // Check if role already has this permission
        var existingRolePermission = await _rolePermissionRepository.GetByRoleAndPermissionAsync(req.RoleId, req.PermissionId);
        if (existingRolePermission != null)
        {
            if (existingRolePermission.IsActive)
            {
                throw new InvalidOperationException($"Role already has permission {permission.Name}");
            }
            else
            {
                // Reactivate the permission
                existingRolePermission.IsActive = true;
                existingRolePermission.AssignedAt = DateTime.UtcNow;
                await _rolePermissionRepository.UpdateAsync(existingRolePermission);
                
                _logger.LogInformation("Reactivated permission {PermissionName} for role {RoleName}", permission.Name, role.Name);
                
                return new GrantPermissionToRoleResponse
                {
                    RoleId = req.RoleId,
                    PermissionId = req.PermissionId,
                    Message = $"Permission {permission.Name} reactivated for role {role.Name}"
                };
            }
        }

        // Create new role permission assignment
        var rolePermission = new RolePermission
        {

            RoleId = req.RoleId,
            PermissionId = req.PermissionId,
            AssignedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _rolePermissionRepository.CreateAsync(rolePermission);

        _logger.LogInformation("Granted permission {PermissionName} to role {RoleName}", permission.Name, role.Name);

        return new GrantPermissionToRoleResponse
        {
            RoleId = req.RoleId,
            PermissionId = req.PermissionId,
            Message = $"Permission {permission.Name} granted to role {role.Name} successfully"
        };
    }
}
