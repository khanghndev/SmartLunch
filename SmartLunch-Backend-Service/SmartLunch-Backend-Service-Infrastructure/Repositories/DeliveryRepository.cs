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
            .Include(d => d.AssignedStaff)
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

    public async Task<(List<Delivery> Deliveries, int TotalCount)> GetForManagerAsync(
        int page,
        int pageSize,
        string? status = null,
        DateOnly? scheduledOn = null,
        string? searchTerm = null,
        bool? unassignedOnly = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Deliveries
            .Include(d => d.Order)
                .ThenInclude(o => o!.OrderItems)
            .Include(d => d.Order)
                .ThenInclude(o => o!.Contract)
                    .ThenInclude(c => c!.Organization)
            .Include(d => d.AssignedStaff)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            var s = status.Trim().ToLowerInvariant();
            query = query.Where(d => d.DeliveryStatus.ToLower() == s);
        }

        if (scheduledOn.HasValue)
        {
            var start = VietnamTime.CalendarDateMidnight(scheduledOn.Value);
            var end = start.AddDays(1);
            query = query.Where(d => d.Order != null && d.Order.ScheduledDate >= start && d.Order.ScheduledDate < end);
        }

        if (unassignedOnly == true)
            query = query.Where(d => d.AssignedStaffId == null);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(d =>
                d.DeliveryAddress.Contains(term) ||
                (d.Code != null && d.Code.Contains(term)) ||
                (d.Order != null && d.Order.InvoiceCode != null && d.Order.InvoiceCode.Contains(term)) ||
                (d.Order != null && d.Order.Code != null && d.Order.Code.Contains(term)) ||
                (d.Order != null && d.Order.Contract != null && d.Order.Contract.Organization != null &&
                 d.Order.Contract.Organization.Name.Contains(term)) ||
                (d.AssignedStaff != null && (
                    (d.AssignedStaff.FirstName != null && d.AssignedStaff.FirstName.Contains(term)) ||
                    (d.AssignedStaff.LastName != null && d.AssignedStaff.LastName.Contains(term)) ||
                    d.AssignedStaff.Username.Contains(term))));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var deliveries = await query
            .OrderByDescending(d => d.Order != null ? d.Order.ScheduledDate : d.CreatedAt)
            .ThenByDescending(d => d.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

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
            var start = VietnamTime.CalendarDateMidnight(scheduledOn.Value);
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

    public async Task<(int Pending, int InProgress, int Completed, int Unassigned)> GetManagerStatsAsync(
        DateOnly? scheduledOn = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Deliveries.AsNoTracking().Include(d => d.Order).AsQueryable();
        if (scheduledOn.HasValue)
        {
            var start = VietnamTime.CalendarDateMidnight(scheduledOn.Value);
            var end = start.AddDays(1);
            query = query.Where(d => d.Order != null && d.Order.ScheduledDate >= start && d.Order.ScheduledDate < end);
        }

        var rows = await query.Select(d => new { d.DeliveryStatus, d.AssignedStaffId }).ToListAsync(cancellationToken);
        var pending = rows.Count(x => string.Equals(x.DeliveryStatus, "pending", StringComparison.OrdinalIgnoreCase));
        var inProgress = rows.Count(x =>
        {
            var s = (x.DeliveryStatus ?? "").ToLowerInvariant();
            return s is "received" or "in_transit";
        });
        var completed = rows.Count(x => string.Equals(x.DeliveryStatus, "completed", StringComparison.OrdinalIgnoreCase));
        var unassigned = rows.Count(x =>
            x.AssignedStaffId == null &&
            !string.Equals(x.DeliveryStatus, "completed", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(x.DeliveryStatus, "failed", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(x.DeliveryStatus, "rejected", StringComparison.OrdinalIgnoreCase));

        return (pending, inProgress, completed, unassigned);
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
