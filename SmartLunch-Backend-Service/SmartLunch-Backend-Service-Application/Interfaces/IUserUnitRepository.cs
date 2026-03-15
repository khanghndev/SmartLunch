using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IUserUnitRepository
{
    Task<UserUnit?> GetByIdAsync(Guid id);
    Task<UserUnit?> GetByUserAndUnitAsync(Guid userId, Guid unitId);
    Task<IEnumerable<UserUnit>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<UserUnit>> GetActiveByUserIdAsync(Guid userId);
    Task<IEnumerable<UserUnit>> GetByUnitIdAsync(Guid unitId);
    Task<(List<UserUnit> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, Guid? userId = null, Guid? unitId = null, bool? isActive = null);
    Task<UserUnit> CreateAsync(UserUnit userUnit);
    Task<UserUnit> UpdateAsync(UserUnit userUnit);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> DeleteByUserAndUnitAsync(Guid userId, Guid unitId);
    Task<bool> ExistsByUserAndUnitAsync(Guid userId, Guid unitId);
}
