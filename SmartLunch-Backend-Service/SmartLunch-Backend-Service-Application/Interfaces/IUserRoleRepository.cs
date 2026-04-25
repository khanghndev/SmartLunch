using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IUserRoleRepository
{
    Task<UserRole?> GetByIdAsync(int id);
    Task<UserRole?> GetByUserAndRoleAsync(int userId, int roleId);
    Task<IEnumerable<UserRole>> GetByUserIdAsync(int userId);
    Task<IEnumerable<UserRole>> GetActiveByUserIdAsync(int userId);
    Task<IEnumerable<UserRole>> GetByRoleIdAsync(int roleId);
    Task<UserRole> CreateAsync(UserRole userRole);
    Task<UserRole> UpdateAsync(UserRole userRole);
    Task<bool> DeleteAsync(int id);
    Task<bool> DeleteByUserAndRoleAsync(int userId, int roleId);
    Task<bool> ExistsByUserAndRoleAsync(int userId, int roleId);
    Task<(List<UserRole> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, int? userId = null, int? roleId = null, bool? isActive = null);
}
