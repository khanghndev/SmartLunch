using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IRolePermissionRepository
{
    Task<RolePermission?> GetByIdAsync(Guid id);
    Task<RolePermission?> GetByRoleAndPermissionAsync(Guid roleId, Guid permissionId);
    Task<IEnumerable<RolePermission>> GetByRoleIdAsync(Guid roleId);
    Task<IEnumerable<RolePermission>> GetActiveByRoleIdAsync(Guid roleId);
    Task<IEnumerable<RolePermission>> GetByPermissionIdAsync(Guid permissionId);
    Task<RolePermission> CreateAsync(RolePermission rolePermission);
    Task<RolePermission> UpdateAsync(RolePermission rolePermission);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> DeleteByRoleAndPermissionAsync(Guid roleId, Guid permissionId);
    Task<bool> ExistsByRoleAndPermissionAsync(Guid roleId, Guid permissionId);
    Task<(List<RolePermission> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, Guid? roleId = null, Guid? permissionId = null, bool? isActive = null);
}
