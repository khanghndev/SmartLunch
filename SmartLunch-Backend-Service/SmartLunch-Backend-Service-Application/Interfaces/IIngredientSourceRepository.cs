using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IIngredientSourceRepository
{
    Task<IngredientSource?> GetByIdAsync(int id);
    Task<(List<IngredientSource> IngredientSources, int TotalCount)> GetIngredientSourcesAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        int? partnerId = null,
        int? ingredientId = null);
    Task<IngredientSource> CreateAsync(IngredientSource ingredientSource);
    Task<IngredientSource> UpdateAsync(IngredientSource ingredientSource);
    Task DeleteAsync(IngredientSource ingredientSource);

    Task<IReadOnlyList<IngredientSource>> GetRecentByIngredientIdAsync(
        int ingredientId,
        int take,
        CancellationToken cancellationToken = default);
}
