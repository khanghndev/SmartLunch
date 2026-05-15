using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IDishCategoryRepository
{
    Task<List<DishCategory>> GetAllOrderedAsync(CancellationToken cancellationToken = default);
}
