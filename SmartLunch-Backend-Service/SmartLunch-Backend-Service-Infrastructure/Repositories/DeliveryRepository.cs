using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class DeliveryRepository : IDeliveryRepository
{
    private readonly SmartLunchDBContext _context;

    public DeliveryRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<Delivery?> GetByIdAsync(int id)
    {
        return await _context.Deliveries
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Delivery?> GetByIdWithOrderAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Deliveries
            .Include(d => d.Order)
                .ThenInclude(o => o.OrderItems)
            .Include(d => d.Order)
                .ThenInclude(o => o.Contract)
                    .ThenInclude(c => c.Organization)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<(List<Delivery> Deliveries, int TotalCount)> GetDeliveriesAsync(int page, int pageSize, string? searchTerm = null)
    {
        var query = _context.Deliveries.AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(e =>
                e.DeliveryAddress.Contains(searchTerm) ||
                e.DeliveryStatus.Contains(searchTerm) ||
                (e.Notes != null && e.Notes.Contains(searchTerm)));
        }

        var totalCount = await query.CountAsync();

        var deliveries = await query
            .OrderBy(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (deliveries, totalCount);
    }

    public async Task<(List<Delivery> Deliveries, int TotalCount)> GetForShipperAsync(
        int shipperUserId,
        int page,
        int pageSize,
        string? status = null,
        DateOnly? scheduledOn = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Deliveries
            .Include(d => d.Order)
                .ThenInclude(o => o.OrderItems)
            .AsQueryable();

        // Shipper can see unassigned pending tasks and tasks assigned to them.
        query = query.Where(d =>
            (d.AssignedStaffId == null && d.DeliveryStatus == "pending") ||
            d.AssignedStaffId == shipperUserId);

        if (!string.IsNullOrWhiteSpace(status))
        {
            var s = status.Trim().ToLowerInvariant();
            query = query.Where(d => d.DeliveryStatus.ToLower() == s);
        }

        if (scheduledOn.HasValue)
        {
            var start = DateTime.SpecifyKind(scheduledOn.Value.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
            var end = start.AddDays(1);
            query = query.Where(d => d.Order != null && d.Order.ScheduledDate >= start && d.Order.ScheduledDate < end);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var deliveries = await query
            .OrderBy(d => d.Order != null ? d.Order.ScheduledDate : d.CreatedAt)
            .ThenBy(d => d.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (deliveries, totalCount);
    }

    public Task UpdateAsync(Delivery delivery, CancellationToken cancellationToken = default)
    {
        _context.Deliveries.Update(delivery);
        return Task.CompletedTask;
    }

    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
