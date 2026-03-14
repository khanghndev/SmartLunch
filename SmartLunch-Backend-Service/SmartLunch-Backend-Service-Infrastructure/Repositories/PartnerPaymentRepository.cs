using Microsoft.EntityFrameworkCore;
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

    public async Task<PartnerPayment?> GetByIdAsync(Guid id)
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
}
