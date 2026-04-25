using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class UnitRepository : IUnitRepository
{
    private readonly SmartLunchDBContext _context;

    public UnitRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<Unit?> GetByIdAsync(int id)
    {
        return await _context.Units
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<(List<Unit> Units, int TotalCount)> GetUnitsAsync(int page, int pageSize, string? searchTerm = null, bool? isActive = null)
    {
        var query = _context.Units.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(u =>
                u.Name.Contains(searchTerm) ||
                (u.Address != null && u.Address.Contains(searchTerm)) ||
                (u.ContactPerson != null && u.ContactPerson.Contains(searchTerm)) ||
                (u.ContactEmail != null && u.ContactEmail.Contains(searchTerm)));
        }

        if (isActive.HasValue)
        {
            query = query.Where(u => u.IsActive == isActive.Value);
        }

        var totalCount = await query.CountAsync();

        var units = await query
            .OrderBy(u => u.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (units, totalCount);
    }

    public async Task<Dictionary<int, string>> GetNamesByIdsAsync(
        IEnumerable<int> unitIds,
        CancellationToken cancellationToken = default)
    {
        var idList = unitIds.Distinct().ToList();
        if (idList.Count == 0)
            return new Dictionary<int, string>();

        return await _context.Units
            .AsNoTracking()
            .Where(u => idList.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.Name, cancellationToken);
    }
}
