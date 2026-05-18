using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.RolePermissions;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.RolePermissions.CreateRolePermission;

public class CreateRolePermissionCommandHandler : IRequestHandler<CreateRolePermissionCommand, CreateRolePermissionResponse>
{
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;

    public CreateRolePermissionCommandHandler(
        IRolePermissionRepository rolePermissionRepository,
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository)
    {
        _rolePermissionRepository = rolePermissionRepository;
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
    }

    public async Task<CreateRolePermissionResponse> Handle(CreateRolePermissionCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;

        if (await _roleRepository.GetByIdAsync(req.RoleId) == null)
            throw new KeyNotFoundException($"Role with ID {req.RoleId} not found");
        if (await _permissionRepository.GetByIdAsync(req.PermissionId) == null)
            throw new KeyNotFoundException($"Permission with ID {req.PermissionId} not found");
        if (await _rolePermissionRepository.ExistsByRoleAndPermissionAsync(req.RoleId, req.PermissionId))
            throw new InvalidOperationException("Role already has this permission");

        var entity = new RolePermission
        {

            RoleId = req.RoleId,
            PermissionId = req.PermissionId,
            AssignedAt = VietnamTime.Now,
            AssignedBy = req.AssignedBy,
            IsActive = true
        };
        await _rolePermissionRepository.CreateAsync(entity);

        return new CreateRolePermissionResponse
        {
            Id = entity.Id,
            RoleId = entity.RoleId,
            PermissionId = entity.PermissionId,
            Message = "Role permission created successfully"
        };
    }
}
