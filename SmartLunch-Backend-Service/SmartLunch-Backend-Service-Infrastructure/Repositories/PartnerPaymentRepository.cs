using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class PartnerPaymentRepository : IPartnerPaymentRepository
{
    private readonly SmartLunchDBContext _context;

    public PartnerPaymentRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<PartnerPayment?> GetByIdAsync(int id)
    {
        return await _context.PartnerPayments
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<(List<PartnerPayment> PartnerPayments, int TotalCount)> GetPartnerPaymentsAsync(int page, int pageSize, string? searchTerm = null)
    {
        var query = _context.PartnerPayments.AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(e =>
                e.Method.Contains(searchTerm) ||
                e.Status.Contains(searchTerm));
        }

        var totalCount = await query.CountAsync();

        var partnerPayments = await query
            .OrderBy(e => e.PaymentDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (partnerPayments, totalCount);
    }

    public async Task<PartnerPayment> CreateAsync(PartnerPayment entity, CancellationToken cancellationToken = default)
    {
        await _context.PartnerPayments.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<decimal> GetCompletedTotalByContractAsync(int contractId, CancellationToken cancellationToken = default) =>
        await _context.PartnerPayments
            .AsNoTracking()
            .Where(p => p.ContractId == contractId && p.Status == PartnerPaymentStatus.Completed)
            .SumAsync(p => p.Amount, cancellationToken);

    public async Task<Dictionary<int, decimal>> GetCompletedTotalsByContractIdsAsync(
        IReadOnlyList<int> contractIds,
        CancellationToken cancellationToken = default)
    {
        if (contractIds.Count == 0) return new Dictionary<int, decimal>();
        return await _context.PartnerPayments
            .AsNoTracking()
            .Where(p => contractIds.Contains(p.ContractId) && p.Status == PartnerPaymentStatus.Completed)
            .GroupBy(p => p.ContractId)
            .Select(g => new { ContractId = g.Key, Paid = g.Sum(x => x.Amount) })
            .ToDictionaryAsync(x => x.ContractId, x => x.Paid, cancellationToken);
    }
}
