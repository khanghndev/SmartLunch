using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IDebtAnalyticsRepository
{
    /// <summary>Per organization: tổng tiền đơn (chưa hủy), tổng đã thu (Payment paid), số đơn.</summary>
    Task<Dictionary<int, (decimal Billed, decimal Paid, int OrderCount)>> GetOrganizationBilledAndPaidAsync(
        int? organizationId,
        CancellationToken cancellationToken = default);

    /// <summary>Per partner: tổng giá trị hợp đồng (TotalValue) và tổng đã chi (PartnerPayment completed).</summary>
    Task<Dictionary<int, (decimal ContractValue, decimal PaidOut)>> GetPartnerContractAndPaidAsync(
        int? partnerId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Payment>> GetCustomerPaymentsInRangeAsync(
        DateTime rangeStart,
        DateTime rangeEndExclusive,
        int? organizationId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PartnerPayment>> GetSupplierPaymentsInRangeAsync(
        DateTime rangeStart,
        DateTime rangeEndExclusive,
        int? partnerId,
        CancellationToken cancellationToken = default);
}
