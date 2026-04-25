using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IUserPermissionRepository
{
    Task<UserPermission?> GetByIdAsync(int id);
    Task<UserPermission?> GetByUserAndPermissionAsync(int userId, int permissionId);
    Task<IEnumerable<UserPermission>> GetByUserIdAsync(int userId);
    Task<IEnumerable<UserPermission>> GetActiveByUserIdAsync(int userId);
    Task<IEnumerable<UserPermission>> GetByPermissionIdAsync(int permissionId);
    Task<UserPermission> CreateAsync(UserPermission userPermission);
    Task<UserPermission> UpdateAsync(UserPermission userPermission);
    Task<bool> DeleteAsync(int id);
    Task<bool> DeleteByUserAndPermissionAsync(int userId, int permissionId);
    Task<bool> ExistsByUserAndPermissionAsync(int userId, int permissionId);
    Task<(List<UserPermission> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, int? userId = null, int? permissionId = null, bool? isActive = null);
}
