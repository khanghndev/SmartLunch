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
            .Include(o => o.Organization)
            .Where(o => o.ScheduledDate >= rangeStart && o.ScheduledDate < rangeEndExclusive);

        if (!includeCancelledOrders)
            query = query.Where(o => o.Status != OrderLifecycleStatus.Cancelled);

        return await query.AsNoTracking().ToListAsync(cancellationToken);
    }
}
