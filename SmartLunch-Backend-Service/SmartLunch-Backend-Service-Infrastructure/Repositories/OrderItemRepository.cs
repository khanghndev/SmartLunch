using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class OrderItemRepository : IOrderItemRepository
{
    private readonly SmartLunchDBContext _context;

    public OrderItemRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<OrderItem?> GetByIdAsync(int id)
    {
        return await _context.OrderItems
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<(List<OrderItem> OrderItems, int TotalCount)> GetOrderItemsAsync(int page, int pageSize, string? searchTerm = null)
    {
        var query = _context.OrderItems.AsQueryable();

        var totalCount = await query.CountAsync();

        var orderItems = await query
            .OrderBy(e => e.OrderId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (orderItems, totalCount);
    }
}
