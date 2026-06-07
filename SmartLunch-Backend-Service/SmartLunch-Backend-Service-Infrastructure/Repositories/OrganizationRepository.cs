using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class OrganizationRepository : IOrganizationRepository
{
    private readonly SmartLunchDBContext _context;

    public OrganizationRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<Organization?> GetByIdAsync(int id)
    {
        return await _context.Organizations
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<(List<Organization> Organizations, int TotalCount)> GetOrganizationsAsync(int page, int pageSize, string? searchTerm = null, bool? isActive = null)
    {
        var query = _context.Organizations.AsQueryable();

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

        var organizations = await query
            .OrderBy(u => u.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (organizations, totalCount);
    }

    public async Task<Dictionary<int, string>> GetNamesByIdsAsync(
        IEnumerable<int> organizationIds,
        CancellationToken cancellationToken = default)
    {
        var idList = organizationIds.Distinct().ToList();
        if (idList.Count == 0)
            return new Dictionary<int, string>();

        return await _context.Organizations
            .AsNoTracking()
            .Where(u => idList.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.Name, cancellationToken);
    }

    public async Task<Organization> CreateAsync(Organization organization, CancellationToken cancellationToken = default)
    {
        _context.Organizations.Add(organization);
        await _context.SaveChangesAsync(cancellationToken);
        return organization;
    }

    public async Task UpdateAsync(Organization organization, CancellationToken cancellationToken = default)
    {
        _context.Organizations.Update(organization);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
