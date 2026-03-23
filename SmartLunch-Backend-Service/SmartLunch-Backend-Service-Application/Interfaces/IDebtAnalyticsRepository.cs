using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IDebtAnalyticsRepository
{
    /// <summary>Per unit: tổng tiền đơn (chưa hủy), tổng đã thu (Payment paid), số đơn.</summary>
    Task<Dictionary<Guid, (decimal Billed, decimal Paid, int OrderCount)>> GetUnitBilledAndPaidAsync(
        Guid? unitId,
        CancellationToken cancellationToken = default);

    /// <summary>Per partner: tổng giá trị hợp đồng (TotalValue) và tổng đã chi (PartnerPayment completed).</summary>
    Task<Dictionary<Guid, (decimal ContractValue, decimal PaidOut)>> GetPartnerContractAndPaidAsync(
        Guid? partnerId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Payment>> GetCustomerPaymentsInRangeAsync(
        DateTime rangeStart,
        DateTime rangeEndExclusive,
        Guid? unitId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PartnerPayment>> GetSupplierPaymentsInRangeAsync(
        DateTime rangeStart,
        DateTime rangeEndExclusive,
        Guid? partnerId,
        CancellationToken cancellationToken = default);
}
