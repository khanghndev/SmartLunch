using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IPartnerPaymentRepository
{
    Task<PartnerPayment?> GetByIdAsync(int id);
    Task<(List<PartnerPayment> PartnerPayments, int TotalCount)> GetPartnerPaymentsAsync(int page, int pageSize, string? searchTerm = null);
    Task<PartnerPayment> CreateAsync(PartnerPayment entity, CancellationToken cancellationToken = default);
    Task<decimal> GetCompletedTotalByContractAsync(int contractId, CancellationToken cancellationToken = default);
    Task<Dictionary<int, decimal>> GetCompletedTotalsByContractIdsAsync(
        IReadOnlyList<int> contractIds,
        CancellationToken cancellationToken = default);
}
