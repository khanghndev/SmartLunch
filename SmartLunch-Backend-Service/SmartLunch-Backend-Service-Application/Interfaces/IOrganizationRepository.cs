using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IOrganizationRepository
{
    Task<Organization?> GetByIdAsync(int id);
    Task<(List<Organization> Organizations, int TotalCount)> GetOrganizationsAsync(int page, int pageSize, string? searchTerm = null, bool? isActive = null);
    Task<Dictionary<int, string>> GetNamesByIdsAsync(IEnumerable<int> organizationIds, CancellationToken cancellationToken = default);
    Task<Organization> CreateAsync(Organization organization, CancellationToken cancellationToken = default);
    Task UpdateAsync(Organization organization, CancellationToken cancellationToken = default);
}
