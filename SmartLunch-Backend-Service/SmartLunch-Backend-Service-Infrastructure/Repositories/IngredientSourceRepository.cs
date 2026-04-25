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

    public async Task<IngredientSource?> GetByIdAsync(int id)
    {
        return await _context.IngredientSources
            .Include(e => e.Ingredient)
            .Include(e => e.Partner)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<(List<IngredientSource> IngredientSources, int TotalCount)> GetIngredientSourcesAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        int? partnerId = null,
        int? ingredientId = null)
    {
        var query = _context.IngredientSources
            .Include(e => e.Ingredient)
            .Include(e => e.Partner)
            .AsQueryable();

        if (partnerId.HasValue)
            query = query.Where(e => e.PartnerId == partnerId.Value);
        if (ingredientId.HasValue)
            query = query.Where(e => e.IngredientId == ingredientId.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(e =>
                (e.BatchNumber != null && e.BatchNumber.Contains(term)) ||
                (e.OriginDetails != null && e.OriginDetails.Contains(term)) ||
                (e.Certification != null && e.Certification.Contains(term)) ||
                (e.Ingredient != null && e.Ingredient.Name.Contains(term)) ||
                (e.Partner != null && e.Partner.LegalName.Contains(term)));
        }

        var totalCount = await query.CountAsync();

        var ingredientSources = await query
            .OrderByDescending(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (ingredientSources, totalCount);
    }

    public async Task<IngredientSource> CreateAsync(IngredientSource ingredientSource)
    {
        _context.IngredientSources.Add(ingredientSource);
        await _context.SaveChangesAsync();
        return ingredientSource;
    }

    public async Task<IngredientSource> UpdateAsync(IngredientSource ingredientSource)
    {
        _context.IngredientSources.Update(ingredientSource);
        await _context.SaveChangesAsync();
        return ingredientSource;
    }

    public async Task DeleteAsync(IngredientSource ingredientSource)
    {
        _context.IngredientSources.Remove(ingredientSource);
        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<IngredientSource>> GetRecentByIngredientIdAsync(
        int ingredientId,
        int take,
        CancellationToken cancellationToken = default)
    {
        if (take < 1)
            take = 20;
        if (take > 100)
            take = 100;

        return await _context.IngredientSources
            .AsNoTracking()
            .Include(e => e.Partner)
            .Where(e => e.IngredientId == ingredientId)
            .OrderByDescending(e => e.CreatedAt)
            .Take(take)
            .ToListAsync(cancellationToken);
    }
}
