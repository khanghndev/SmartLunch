using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IPartnerRepository
{
    Task<Partner?> GetByIdAsync(int id);
    Task<Partner?> GetByIdWithContractsAsync(int id);
    Task<(List<Partner> Partners, int TotalCount)> GetPartnersAsync(int page, int pageSize, string? searchTerm = null, bool? isActive = null);
    Task<bool> ExistsByTaxIdAsync(string taxId, int? excludePartnerId = null);
    Task<Partner> CreateAsync(Partner partner);
    Task<Partner> UpdateAsync(Partner partner);
    Task<Dictionary<int, string>> GetLegalNamesByIdsAsync(IEnumerable<int> partnerIds, CancellationToken cancellationToken = default);
}