using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IDishValueRepository
{
    Task<List<DishValue>> GetActiveOrderedAsync(CancellationToken cancellationToken = default);
}
