using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class DishRepository : IDishRepository
{
    private readonly SmartLunchDBContext _context;

    public DishRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<Dish?> GetByIdAsync(Guid id)
    {
        return await _context.Dishes
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<(List<Dish> Dishes, int TotalCount)> GetDishesAsync(int page, int pageSize, string? searchTerm = null, bool? isActive = null, string? category = null)
    {
        var query = _context.Dishes.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(d =>
                d.Name.Contains(searchTerm) ||
                (d.Description != null && d.Description.Contains(searchTerm)) ||
                (d.Category != null && d.Category.Contains(searchTerm)) ||
                (d.DietaryLabel != null && d.DietaryLabel.Contains(searchTerm)));
        }

        if (isActive.HasValue)
        {
            query = query.Where(d => d.IsActive == isActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(d => d.Category != null && d.Category == category);
        }

        var totalCount = await query.CountAsync();

        var dishes = await query
            .OrderBy(d => d.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (dishes, totalCount);
    }
}
