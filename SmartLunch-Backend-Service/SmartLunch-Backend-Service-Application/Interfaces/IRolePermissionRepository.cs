using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IRolePermissionRepository
{
    Task<RolePermission?> GetByIdAsync(int id);
    Task<RolePermission?> GetByRoleAndPermissionAsync(int roleId, int permissionId);
    Task<IEnumerable<RolePermission>> GetByRoleIdAsync(int roleId);
    Task<IEnumerable<RolePermission>> GetActiveByRoleIdAsync(int roleId);
    Task<IEnumerable<RolePermission>> GetByPermissionIdAsync(int permissionId);
    Task<RolePermission> CreateAsync(RolePermission rolePermission);
    Task<RolePermission> UpdateAsync(RolePermission rolePermission);
    Task<bool> DeleteAsync(int id);
    Task<bool> DeleteByRoleAndPermissionAsync(int roleId, int permissionId);
    Task<bool> ExistsByRoleAndPermissionAsync(int roleId, int permissionId);
    Task<(List<RolePermission> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, int? roleId = null, int? permissionId = null, bool? isActive = null);
}
