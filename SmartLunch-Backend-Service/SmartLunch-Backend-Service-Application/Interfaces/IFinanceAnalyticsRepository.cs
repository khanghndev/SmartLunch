using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IFinanceAnalyticsRepository
{
    Task<IReadOnlyList<Payment>> GetPaidPaymentsInRangeAsync(DateTime rangeStart, DateTime rangeEndExclusive, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Transaction>> GetTransactionsInRangeAsync(DateTime rangeStart, DateTime rangeEndExclusive, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Order>> GetOrdersWithPaymentsForReconciliationAsync(
        DateTime rangeStart,
        DateTime rangeEndExclusive,
        bool includeCancelledOrders,
        CancellationToken cancellationToken = default);

    Task<ContractFinanceSettlementSnapshot?> GetContractFinanceSettlementSnapshotAsync(
        int contractId,
        DateTime? scheduledFromUtc,
        DateTime? scheduledToExclusiveUtc,
        DateTime? supplierPaymentFromUtc,
        DateTime? supplierPaymentToExclusiveUtc,
        bool includeCancelledOrders,
        bool loadSupplierPayments = true,
        CancellationToken cancellationToken = default);
}
