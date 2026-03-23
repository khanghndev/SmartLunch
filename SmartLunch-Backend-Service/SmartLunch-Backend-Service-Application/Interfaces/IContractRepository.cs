using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IContractRepository
{
    Task<Contract?> GetByIdAsync(Guid id);
    Task<(List<Contract> Contracts, int TotalCount)> GetContractsAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        Guid? partnerId = null);
    Task<bool> ExistsContractNumberForPartnerAsync(Guid partnerId, string contractNumber, Guid? excludeContractId = null);
    Task<int> CountPartnerPaymentsAsync(Guid contractId);
    Task<Contract> CreateAsync(Contract contract);
    Task<Contract> UpdateAsync(Contract contract);
    Task DeleteAsync(Contract contract);
}
