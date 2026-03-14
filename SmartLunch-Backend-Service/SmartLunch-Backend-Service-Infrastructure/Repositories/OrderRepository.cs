using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly SmartLunchDBContext _context;

    public OrderRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(Guid id)
    {
        return await _context.Orders
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<(List<Order> Orders, int TotalCount)> GetOrdersAsync(int page, int pageSize, string? searchTerm = null)
    {
        var query = _context.Orders.AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(e =>
                e.Status.Contains(searchTerm) ||
                e.PaymentStatus.Contains(searchTerm));
        }

        var totalCount = await query.CountAsync();

        var orders = await query
            .OrderBy(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (orders, totalCount);
    }
}
