using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly SmartLunchDBContext _context;

    public InventoryRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<Inventory?> GetByIdAsync(Guid id)
    {
        return await _context.Inventories
            .FirstOrDefaultAsync(e => e.IngredientId == id);
    }

    public async Task<(List<Inventory> Inventories, int TotalCount)> GetInventoriesAsync(int page, int pageSize, string? searchTerm = null)
    {
        var query = _context.Inventories.AsQueryable();

        var totalCount = await query.CountAsync();

        var inventories = await query
            .OrderBy(e => e.LastUpdated)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (inventories, totalCount);
    }
}
