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
    Task<bool> ExistsContractNumberForPartnerAsync(int partnerId, string contractNumber, int? excludeContractId = null);
    Task<int> CountPartnerPaymentsAsync(int contractId);
    Task<Contract> CreateAsync(Contract contract);
    Task<Contract> UpdateAsync(Contract contract);
    Task DeleteAsync(Contract contract);
}
