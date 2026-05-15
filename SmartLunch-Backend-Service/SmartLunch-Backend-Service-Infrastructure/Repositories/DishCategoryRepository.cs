using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class DishCategoryRepository : IDishCategoryRepository
{
    private readonly SmartLunchDBContext _context;

    public DishCategoryRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public Task<List<DishCategory>> GetAllOrderedAsync(CancellationToken cancellationToken = default)
    {
        return _context.DishCategories
            .AsNoTracking()
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }
}
