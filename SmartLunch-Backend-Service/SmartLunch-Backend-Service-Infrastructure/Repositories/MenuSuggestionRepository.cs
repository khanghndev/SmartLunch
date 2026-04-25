using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class MenuSuggestionRepository : IMenuSuggestionRepository
{
    private readonly SmartLunchDBContext _context;

    public MenuSuggestionRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<MenuSuggestion?> GetByIdAsync(int id)
    {
        return await _context.MenuSuggestions
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<(List<MenuSuggestion> MenuSuggestions, int TotalCount)> GetMenuSuggestionsAsync(int page, int pageSize, string? searchTerm = null)
    {
        var query = _context.MenuSuggestions.AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(e =>
                e.SuggestionText.Contains(searchTerm) ||
                (e.AlgorithmVersion != null && e.AlgorithmVersion.Contains(searchTerm)));
        }

        var totalCount = await query.CountAsync();

        var menuSuggestions = await query
            .OrderBy(e => e.GeneratedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (menuSuggestions, totalCount);
    }

    public async Task AddAsync(MenuSuggestion menuSuggestion, CancellationToken cancellationToken = default)
    {
        await _context.MenuSuggestions.AddAsync(menuSuggestion, cancellationToken);
    }

    public Task CommitAsync() => _context.SaveChangesAsync();
}
