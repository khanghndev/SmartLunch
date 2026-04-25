using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly SmartLunchDBContext _context;

    public PaymentRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<Payment?> GetByIdAsync(int id)
    {
        return await _context.Payments
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<(List<Payment> Payments, int TotalCount)> GetPaymentsAsync(int page, int pageSize, string? searchTerm = null)
    {
        var query = _context.Payments.AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(e =>
                e.Method.Contains(searchTerm) ||
                e.Status.Contains(searchTerm));
        }

        var totalCount = await query.CountAsync();

        var payments = await query
            .OrderBy(e => e.PaymentDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (payments, totalCount);
    }
}
