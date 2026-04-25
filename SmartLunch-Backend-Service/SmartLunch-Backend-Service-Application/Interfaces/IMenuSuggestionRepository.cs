using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IMenuSuggestionRepository
{
    Task<MenuSuggestion?> GetByIdAsync(int id);
    Task<(List<MenuSuggestion> MenuSuggestions, int TotalCount)> GetMenuSuggestionsAsync(int page, int pageSize, string? searchTerm = null);

    Task AddAsync(MenuSuggestion menuSuggestion, CancellationToken cancellationToken = default);
    Task CommitAsync();
}
