using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IUserRoleRepository
{
    Task<UserRole?> GetByIdAsync(Guid id);
    Task<UserRole?> GetByUserAndRoleAsync(Guid userId, Guid roleId);
    Task<IEnumerable<UserRole>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<UserRole>> GetActiveByUserIdAsync(Guid userId);
    Task<IEnumerable<UserRole>> GetByRoleIdAsync(Guid roleId);
    Task<UserRole> CreateAsync(UserRole userRole);
    Task<UserRole> UpdateAsync(UserRole userRole);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> DeleteByUserAndRoleAsync(Guid userId, Guid roleId);
    Task<bool> ExistsByUserAndRoleAsync(Guid userId, Guid roleId);
}
