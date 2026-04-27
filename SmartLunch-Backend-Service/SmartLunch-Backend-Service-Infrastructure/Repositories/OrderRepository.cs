using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly SmartLunchDBContext _context;

    public OrderRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _context.Orders
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Order?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.Organization)
            .Include(o => o.CreatedBySalesUser)
            .Include(o => o.OrderItems).ThenInclude(i => i.Dish)
            .Include(o => o.Deliveries)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<(List<Order> Orders, int TotalCount)> GetOrdersAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        DateOnly? scheduledOn = null,
        string? status = null)
    {
        var query = _context.Orders
            .Include(o => o.Organization)
            .Include(o => o.OrderItems).ThenInclude(i => i.Dish)
            .AsQueryable();

        if (scheduledOn.HasValue)
        {
            var start = scheduledOn.Value.ToDateTime(TimeOnly.MinValue);
            var end = start.AddDays(1);
            query = query.Where(o => o.ScheduledDate >= start && o.ScheduledDate < end);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            var s = status.Trim().ToLowerInvariant();
            query = query.Where(o => o.Status == s);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(e =>
                e.Status.Contains(term) ||
                e.PaymentStatus.Contains(term) ||
                (e.Organization != null && e.Organization.Name.Contains(term)));
        }

        var totalCount = await query.CountAsync();

        var orders = await query
            .OrderBy(e => e.ScheduledDate)
            .ThenBy(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (orders, totalCount);
    }

    public async Task<List<MealStatisticItemDto>> GetMealStatisticsAsync(
        DateTime? startDate,
        DateTime? endDate,
        int? organizationId)
    {
        var query = _context.Orders
            .Include(o => o.Organization)
            .Include(o => o.OrderItems)
            .Where(o => o.Status != "cancelled")
            .AsQueryable();

        if (startDate.HasValue)
            query = query.Where(o => o.ScheduledDate >= startDate.Value);
        
        if (endDate.HasValue)
            query = query.Where(o => o.ScheduledDate <= endDate.Value);

        if (organizationId.HasValue)
            query = query.Where(o => o.OrganizationId == organizationId.Value);

        var orders = await query.ToListAsync();

        var result = orders
            .SelectMany(o => o.OrderItems.Select(i => new { Order = o, Item = i }))
            .GroupBy(x => new 
            { 
                Date = DateOnly.FromDateTime(x.Order.ScheduledDate),
                MealSlot = x.Order.ScheduledDate.TimeOfDay.Hours < 15 ? "Lunch" : "Dinner",
                OrganizationId = x.Order.OrganizationId,
                OrganizationName = x.Order.Organization?.Name ?? "Unknown"
            })
            .Select(g => new MealStatisticItemDto
            {
                Date = g.Key.Date,
                MealSlot = g.Key.MealSlot,
                OrganizationId = g.Key.OrganizationId,
                OrganizationName = g.Key.OrganizationName,
                TotalMeals = g.Sum(x => x.Item.Quantity),
                TotalAmount = g.Sum(x => x.Item.TotalPrice)
            })
            .OrderByDescending(x => x.Date)
            .ThenBy(x => x.OrganizationName)
            .ToList();

        return result;
    }

    public Task<bool> InvoiceCodeExistsAsync(string invoiceCode, CancellationToken cancellationToken = default)
    {
        return _context.Orders.AnyAsync(o => o.InvoiceCode == invoiceCode, cancellationToken);
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        await _context.Orders.AddAsync(order, cancellationToken);
    }

    public Task CommitAsync() => _context.SaveChangesAsync();
}
