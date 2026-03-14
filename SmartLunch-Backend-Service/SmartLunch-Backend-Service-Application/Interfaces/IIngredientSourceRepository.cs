using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IIngredientSourceRepository
{
    Task<IngredientSource?> GetByIdAsync(Guid id);
    Task<(List<IngredientSource> IngredientSources, int TotalCount)> GetIngredientSourcesAsync(int page, int pageSize, string? searchTerm = null);
}
