using MediatR;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.RolePermissions.DeleteRolePermission;

public class DeleteRolePermissionCommandHandler : IRequestHandler<DeleteRolePermissionCommand, bool>
{
    private readonly IRolePermissionRepository _rolePermissionRepository;

    public DeleteRolePermissionCommandHandler(IRolePermissionRepository rolePermissionRepository)
    {
        _rolePermissionRepository = rolePermissionRepository;
    }

    public async Task<bool> Handle(DeleteRolePermissionCommand request, CancellationToken cancellationToken)
    {
        var deleted = await _rolePermissionRepository.DeleteAsync(request.Id);
        if (!deleted)
            throw new KeyNotFoundException($"RolePermission with ID {request.Id} not found");
        return true;
    }
}
