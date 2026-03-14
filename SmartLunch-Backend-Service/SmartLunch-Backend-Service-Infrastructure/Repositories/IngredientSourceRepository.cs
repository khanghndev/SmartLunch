using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class IngredientSourceRepository : IIngredientSourceRepository
{
    private readonly SmartLunchDBContext _context;

    public IngredientSourceRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<IngredientSource?> GetByIdAsync(Guid id)
    {
        return await _context.IngredientSources
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<(List<IngredientSource> IngredientSources, int TotalCount)> GetIngredientSourcesAsync(int page, int pageSize, string? searchTerm = null)
    {
        var query = _context.IngredientSources.AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(e =>
                (e.BatchNumber != null && e.BatchNumber.Contains(searchTerm)) ||
                (e.OriginDetails != null && e.OriginDetails.Contains(searchTerm)) ||
                (e.Certification != null && e.Certification.Contains(searchTerm)));
        }

        var totalCount = await query.CountAsync();

        var ingredientSources = await query
            .OrderBy(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (ingredientSources, totalCount);
    }
}
