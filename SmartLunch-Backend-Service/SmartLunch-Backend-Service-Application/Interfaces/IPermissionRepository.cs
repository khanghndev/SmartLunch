using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IPermissionRepository
{
    Task<Permission?> GetByIdAsync(Guid id);
    Task<Permission?> GetByNameAsync(string name);
    Task<IEnumerable<Permission>> GetAllAsync();
    Task<IEnumerable<Permission>> GetActivePermissionsAsync();
    Task<IEnumerable<Permission>> GetByResourceAsync(string resource);
    Task<IEnumerable<Permission>> GetByActionAsync(string action);
    Task<Permission> CreateAsync(Permission permission);
    Task<Permission> UpdateAsync(Permission permission);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsByNameAsync(string name);
    Task<bool> ExistsByIdAsync(Guid id);
}
