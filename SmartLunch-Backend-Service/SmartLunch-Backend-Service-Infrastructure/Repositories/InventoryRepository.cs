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

    public async Task<Inventory?> GetByIdAsync(int id)
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

    public async Task<IReadOnlyList<Inventory>> GetLowStockForActiveIngredientsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Inventories
            .AsNoTracking()
            .Include(i => i.Ingredient)
            .Where(i =>
                i.Ingredient.IsActive
                && i.ReorderLevel != null
                && i.QuantityAvailable <= i.ReorderLevel)
            .OrderBy(i => i.QuantityAvailable)
            .ThenBy(i => i.Ingredient.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Inventory?> GetByIngredientIdWithIngredientAsync(
        int ingredientId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Inventories
            .AsNoTracking()
            .Include(i => i.Ingredient)
            .ThenInclude(ing => ing.DefaultSupplier)
            .FirstOrDefaultAsync(i => i.IngredientId == ingredientId, cancellationToken);
    }
}
