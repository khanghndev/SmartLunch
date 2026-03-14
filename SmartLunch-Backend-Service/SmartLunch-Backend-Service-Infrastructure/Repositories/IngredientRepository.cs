using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class IngredientRepository : IIngredientRepository
{
    private readonly SmartLunchDBContext _context;

    public IngredientRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<Ingredient?> GetByIdAsync(Guid id)
    {
        return await _context.Ingredients
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<(List<Ingredient> Ingredients, int TotalCount)> GetIngredientsAsync(int page, int pageSize, string? searchTerm = null, bool? isActive = null)
    {
        var query = _context.Ingredients.AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(e =>
                e.Name.Contains(searchTerm) ||
                e.Unit.Contains(searchTerm) ||
                (e.Description != null && e.Description.Contains(searchTerm)));
        }
        if (isActive.HasValue)
        {
            query = query.Where(e => e.IsActive == isActive.Value);
        }

        var totalCount = await query.CountAsync();

        var ingredients = await query
            .OrderBy(e => e.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (ingredients, totalCount);
    }
}
