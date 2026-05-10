using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class FinanceAnalyticsRepository : IFinanceAnalyticsRepository
{
    private readonly SmartLunchDBContext _context;

    public FinanceAnalyticsRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Payment>> GetPaidPaymentsInRangeAsync(
        DateTime rangeStart,
        DateTime rangeEndExclusive,
        CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .AsNoTracking()
            .Where(p =>
                p.PaymentDate >= rangeStart &&
                p.PaymentDate < rangeEndExclusive &&
                p.Status == PaymentRecordStatus.Paid)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Transaction>> GetTransactionsInRangeAsync(
        DateTime rangeStart,
        DateTime rangeEndExclusive,
        CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .AsNoTracking()
            .Where(t => t.Date >= rangeStart && t.Date < rangeEndExclusive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Order>> GetOrdersWithPaymentsForReconciliationAsync(
        DateTime rangeStart,
        DateTime rangeEndExclusive,
        bool includeCancelledOrders,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Orders
            .Include(o => o.Payments)
            .Include(o => o.Contract).ThenInclude(c => c.Organization)
            .Where(o => o.ScheduledDate >= rangeStart && o.ScheduledDate < rangeEndExclusive);

        if (!includeCancelledOrders)
            query = query.Where(o => o.Status != OrderLifecycleStatus.Cancelled);

        return await query.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<ContractFinanceSettlementSnapshot?> GetContractFinanceSettlementSnapshotAsync(
        int contractId,
        DateTime? scheduledFromUtc,
        DateTime? scheduledToExclusiveUtc,
        DateTime? supplierPaymentFromUtc,
        DateTime? supplierPaymentToExclusiveUtc,
        bool includeCancelledOrders,
        bool loadSupplierPayments = true,
        CancellationToken cancellationToken = default)
    {
        var contract = await _context.Contracts
            .AsNoTracking()
            .Include(c => c.Partner)
            .Include(c => c.Organization)
            .FirstOrDefaultAsync(c => c.Id == contractId, cancellationToken);

        if (contract == null)
            return null;

        var orderQuery = _context.Orders
            .AsNoTracking()
            .Include(o => o.Payments)
            .Include(o => o.Contract!).ThenInclude(c => c!.Organization)
            .Where(o => o.ContractId == contractId);

        if (!includeCancelledOrders)
            orderQuery = orderQuery.Where(o => o.Status != OrderLifecycleStatus.Cancelled);

        if (scheduledFromUtc.HasValue)
            orderQuery = orderQuery.Where(o => o.ScheduledDate >= scheduledFromUtc.Value);
        if (scheduledToExclusiveUtc.HasValue)
            orderQuery = orderQuery.Where(o => o.ScheduledDate < scheduledToExclusiveUtc.Value);

        var orders = await orderQuery
            .OrderBy(o => o.ScheduledDate)
            .ThenBy(o => o.Id)
            .ToListAsync(cancellationToken);

        IReadOnlyList<PartnerPayment> partnerPayments = Array.Empty<PartnerPayment>();
        if (loadSupplierPayments)
        {
            var ppQuery = _context.PartnerPayments
                .AsNoTracking()
                .Where(pp => pp.ContractId == contractId);

            if (supplierPaymentFromUtc.HasValue)
                ppQuery = ppQuery.Where(pp => pp.PaymentDate >= supplierPaymentFromUtc.Value);
            if (supplierPaymentToExclusiveUtc.HasValue)
                ppQuery = ppQuery.Where(pp => pp.PaymentDate < supplierPaymentToExclusiveUtc.Value);

            partnerPayments = await ppQuery
                .OrderBy(pp => pp.PaymentDate)
                .ThenBy(pp => pp.Id)
                .ToListAsync(cancellationToken);
        }

        return new ContractFinanceSettlementSnapshot(contract, orders, partnerPayments);
    }
}
