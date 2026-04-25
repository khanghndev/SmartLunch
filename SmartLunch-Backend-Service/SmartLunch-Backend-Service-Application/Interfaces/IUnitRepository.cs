using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IUnitRepository
{
    Task<Unit?> GetByIdAsync(int id);
    Task<(List<Unit> Units, int TotalCount)> GetUnitsAsync(int page, int pageSize, string? searchTerm = null, bool? isActive = null);
    Task<Dictionary<int, string>> GetNamesByIdsAsync(IEnumerable<int> unitIds, CancellationToken cancellationToken = default);
}
