using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(Guid id);
    Task<Role?> GetByNameAsync(string name);
    Task<IEnumerable<Role>> GetAllAsync();
    Task<IEnumerable<Role>> GetActiveRolesAsync();
    Task<Role> CreateAsync(Role role);
    Task<Role> UpdateAsync(Role role);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsByNameAsync(string name);
    Task<bool> ExistsByIdAsync(Guid id);
}
