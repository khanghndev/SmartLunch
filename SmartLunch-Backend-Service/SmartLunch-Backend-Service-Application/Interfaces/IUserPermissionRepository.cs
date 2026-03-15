using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IUserPermissionRepository
{
    Task<UserPermission?> GetByIdAsync(Guid id);
    Task<UserPermission?> GetByUserAndPermissionAsync(Guid userId, Guid permissionId);
    Task<IEnumerable<UserPermission>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<UserPermission>> GetActiveByUserIdAsync(Guid userId);
    Task<IEnumerable<UserPermission>> GetByPermissionIdAsync(Guid permissionId);
    Task<UserPermission> CreateAsync(UserPermission userPermission);
    Task<UserPermission> UpdateAsync(UserPermission userPermission);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> DeleteByUserAndPermissionAsync(Guid userId, Guid permissionId);
    Task<bool> ExistsByUserAndPermissionAsync(Guid userId, Guid permissionId);
    Task<(List<UserPermission> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, Guid? userId = null, Guid? permissionId = null, bool? isActive = null);
}
