using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IUserOrganizationRepository
{
    Task<UserOrganization?> GetByIdAsync(int id);
    Task<UserOrganization?> GetByUserAndOrganizationAsync(int userId, int organizationId);
    Task<IEnumerable<UserOrganization>> GetByUserIdAsync(int userId);
    Task<IEnumerable<UserOrganization>> GetActiveByUserIdAsync(int userId);
    Task<IEnumerable<UserOrganization>> GetByOrganizationIdAsync(int organizationId);
    Task<(List<UserOrganization> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, int? userId = null, int? organizationId = null, bool? isActive = null);
    Task<UserOrganization> CreateAsync(UserOrganization userOrganization);
    Task<UserOrganization> UpdateAsync(UserOrganization userOrganization);
    Task<bool> DeleteAsync(int id);
    Task<bool> DeleteByUserAndOrganizationAsync(int userId, int organizationId);
    Task<bool> ExistsByUserAndOrganizationAsync(int userId, int organizationId);
}
