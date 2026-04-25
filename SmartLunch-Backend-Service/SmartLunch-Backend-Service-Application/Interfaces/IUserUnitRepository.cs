using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IUserUnitRepository
{
    Task<UserUnit?> GetByIdAsync(int id);
    Task<UserUnit?> GetByUserAndUnitAsync(int userId, int unitId);
    Task<IEnumerable<UserUnit>> GetByUserIdAsync(int userId);
    Task<IEnumerable<UserUnit>> GetActiveByUserIdAsync(int userId);
    Task<IEnumerable<UserUnit>> GetByUnitIdAsync(int unitId);
    Task<(List<UserUnit> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, int? userId = null, int? unitId = null, bool? isActive = null);
    Task<UserUnit> CreateAsync(UserUnit userUnit);
    Task<UserUnit> UpdateAsync(UserUnit userUnit);
    Task<bool> DeleteAsync(int id);
    Task<bool> DeleteByUserAndUnitAsync(int userId, int unitId);
    Task<bool> ExistsByUserAndUnitAsync(int userId, int unitId);
}
