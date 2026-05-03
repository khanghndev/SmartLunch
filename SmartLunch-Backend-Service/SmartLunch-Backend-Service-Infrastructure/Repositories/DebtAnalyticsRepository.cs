using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;
using ContractStatus = SmartLunch.Backend.Service.Application.Constants.ContractStatus;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class DebtAnalyticsRepository : IDebtAnalyticsRepository
{
    private readonly SmartLunchDBContext _context;

    public DebtAnalyticsRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<Dictionary<int, (decimal Billed, decimal Paid, int OrderCount)>> GetOrganizationBilledAndPaidAsync(
        int? organizationId,
        CancellationToken cancellationToken = default)
    {
        var ordersQuery = _context.Orders
            .AsNoTracking()
            .Where(o => o.Status != OrderLifecycleStatus.Cancelled);

        if (organizationId.HasValue)
            ordersQuery = ordersQuery.Where(o => o.Contract!.OrganizationId == organizationId.Value);

        var billedRows = await ordersQuery
            .GroupBy(o => o.Contract!.OrganizationId!.Value)
            .Select(g => new
            {
                OrganizationId = g.Key,
                Billed = g.Sum(x => x.TotalAmount),
                OrderCount = g.Count()
            })
            .ToListAsync(cancellationToken);

        var billed = billedRows.ToDictionary(x => x.OrganizationId, x => (x.Billed, x.OrderCount));

        var paidQuery =
            from p in _context.Payments.AsNoTracking()
            join o in _context.Orders.AsNoTracking() on p.OrderId equals o.Id
            where p.Status == PaymentRecordStatus.Paid
                  && o.Contract!.OrganizationId != null
                  && o.Status != OrderLifecycleStatus.Cancelled
            select new { o.Contract.OrganizationId, p.Amount };

        if (organizationId.HasValue)
            paidQuery = paidQuery.Where(x => x.OrganizationId == organizationId.Value);

        var paid = await paidQuery
            .GroupBy(x => x.OrganizationId!.Value)
            .Select(g => new { OrganizationId = g.Key, Paid = g.Sum(x => x.Amount) })
            .ToDictionaryAsync(x => x.OrganizationId, x => x.Paid, cancellationToken);

        var unitIds = billed.Keys.Union(paid.Keys).ToHashSet();
        var result = new Dictionary<int, (decimal Billed, decimal Paid, int OrderCount)>();
        foreach (var id in unitIds)
        {
            decimal bAmt = 0;
            var oCnt = 0;
            if (billed.TryGetValue(id, out var bRow))
            {
                bAmt = bRow.Billed;
                oCnt = bRow.OrderCount;
            }

            paid.TryGetValue(id, out var p);
            result[id] = (bAmt, p, oCnt);
        }

        return result;
    }

    public async Task<Dictionary<int, (decimal ContractValue, decimal PaidOut)>> GetPartnerContractAndPaidAsync(
        int? partnerId,
        CancellationToken cancellationToken = default)
    {
        var contractQuery = _context.Contracts
            .AsNoTracking()
            .Where(c => c.Status != ContractStatus.Cancelled);

        if (partnerId.HasValue)
            contractQuery = contractQuery.Where(c => c.PartnerId == partnerId.Value);

        var contractTotals = await contractQuery
            .GroupBy(c => c.PartnerId)
            .Select(g => new
            {
                PartnerId = g.Key,
                Total = g.Sum(c => c.TotalValue ?? 0m)
            })
            .ToDictionaryAsync(x => x.PartnerId, x => x.Total, cancellationToken);

        var payQuery = _context.PartnerPayments
            .AsNoTracking()
            .Where(pp => pp.Status == PartnerPaymentStatus.Completed);

        if (partnerId.HasValue)
            payQuery = payQuery.Where(pp => pp.PartnerId == partnerId.Value);

        var paidOut = await payQuery
            .GroupBy(pp => pp.PartnerId)
            .Select(g => new { PartnerId = g.Key, Paid = g.Sum(x => x.Amount) })
            .ToDictionaryAsync(x => x.PartnerId, x => x.Paid, cancellationToken);

        var ids = contractTotals.Keys.Union(paidOut.Keys).ToHashSet();
        var result = new Dictionary<int, (decimal ContractValue, decimal PaidOut)>();
        foreach (var id in ids)
        {
            contractTotals.TryGetValue(id, out var c);
            paidOut.TryGetValue(id, out var p);
            result[id] = (c, p);
        }

        return result;
    }

    public async Task<IReadOnlyList<Payment>> GetCustomerPaymentsInRangeAsync(
        DateTime rangeStart,
        DateTime rangeEndExclusive,
        int? organizationId,
        CancellationToken cancellationToken = default)
    {
        var q = _context.Payments
            .AsNoTracking()
            .Include(p => p.Order)
            .ThenInclude(o => o.Contract).ThenInclude(c => c.Organization)
            .Include(p => p.Payer)
            .Where(p => p.PaymentDate >= rangeStart && p.PaymentDate < rangeEndExclusive);

        if (organizationId.HasValue)
            q = q.Where(p => p.Order.Contract!.OrganizationId == organizationId.Value);

        return await q
            .OrderByDescending(p => p.PaymentDate)
            .ThenByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PartnerPayment>> GetSupplierPaymentsInRangeAsync(
        DateTime rangeStart,
        DateTime rangeEndExclusive,
        int? partnerId,
        CancellationToken cancellationToken = default)
    {
        var q = _context.PartnerPayments
            .AsNoTracking()
            .Include(pp => pp.Partner)
            .Include(pp => pp.Contract)
            .Where(pp => pp.PaymentDate >= rangeStart && pp.PaymentDate < rangeEndExclusive);

        if (partnerId.HasValue)
            q = q.Where(pp => pp.PartnerId == partnerId.Value);

        return await q
            .OrderByDescending(pp => pp.PaymentDate)
            .ThenByDescending(pp => pp.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
