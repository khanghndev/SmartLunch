using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IPartnerRepository
{
    Task<Partner?> GetByIdAsync(Guid id);
    Task<Partner?> GetByIdWithContractsAsync(Guid id);
    Task<(List<Partner> Partners, int TotalCount)> GetPartnersAsync(int page, int pageSize, string? searchTerm = null, bool? isActive = null);
    Task<bool> ExistsByTaxIdAsync(string taxId, Guid? excludePartnerId = null);
    Task<Partner> CreateAsync(Partner partner);
    Task<Partner> UpdateAsync(Partner partner);
    Task<Dictionary<Guid, string>> GetLegalNamesByIdsAsync(IEnumerable<Guid> partnerIds, CancellationToken cancellationToken = default);
}