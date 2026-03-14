using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IUnitRepository
{
    Task<Unit?> GetByIdAsync(Guid id);
    Task<(List<Unit> Units, int TotalCount)> GetUnitsAsync(int page, int pageSize, string? searchTerm = null, bool? isActive = null);
}
