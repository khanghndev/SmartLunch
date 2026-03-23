using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IIngredientSourceRepository
{
    Task<IngredientSource?> GetByIdAsync(Guid id);
    Task<(List<IngredientSource> IngredientSources, int TotalCount)> GetIngredientSourcesAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        Guid? partnerId = null,
        Guid? ingredientId = null);
    Task<IngredientSource> CreateAsync(IngredientSource ingredientSource);
    Task<IngredientSource> UpdateAsync(IngredientSource ingredientSource);
    Task DeleteAsync(IngredientSource ingredientSource);

    Task<IReadOnlyList<IngredientSource>> GetRecentByIngredientIdAsync(
        Guid ingredientId,
        int take,
        CancellationToken cancellationToken = default);
}
