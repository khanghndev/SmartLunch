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
}
