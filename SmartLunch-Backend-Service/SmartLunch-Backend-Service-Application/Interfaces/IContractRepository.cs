using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IContractRepository
{
    Task<Contract?> GetByIdAsync(int id);
    Task<(List<Contract> Contracts, int TotalCount)> GetContractsAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        int? partnerId = null);
    Task<Contract?> GetActiveForOrganizationAsync(int organizationId, CancellationToken cancellationToken = default);

    /// <summary>Hợp đồng gắn các tổ chức (kênh khách doanh nghiệp tự phục vụ).</summary>
    Task<List<Contract>> GetByOrganizationIdsAsync(
        IReadOnlyList<int> organizationIds,
        CancellationToken cancellationToken = default);
    Task<bool> ExistsContractNumberForPartnerAsync(int partnerId, string contractNumber, int? excludeContractId = null);
    Task<int> CountPartnerPaymentsAsync(int contractId);
    Task<Contract> CreateAsync(Contract contract);
    Task<Contract> UpdateAsync(Contract contract);
    Task DeleteAsync(Contract contract);
}
