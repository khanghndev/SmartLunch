using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class DishValueRepository : IDishValueRepository
{
    private readonly SmartLunchDBContext _context;

    public DishValueRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public Task<List<DishValue>> GetActiveOrderedAsync(CancellationToken cancellationToken = default) =>
        _context.DishValues
            .AsNoTracking()
            .Where(v => v.IsActive)
            .OrderBy(v => v.SortOrder)
            .ThenBy(v => v.Amount)
            .ToListAsync(cancellationToken);
}
