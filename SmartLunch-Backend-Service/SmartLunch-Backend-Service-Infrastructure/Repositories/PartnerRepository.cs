using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class PartnerRepository : IPartnerRepository
{
    private readonly SmartLunchDBContext _context;

    public PartnerRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<Partner?> GetByIdAsync(Guid id)
    {
        return await _context.Partners
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<(List<Partner> Partners, int TotalCount)> GetPartnersAsync(int page, int pageSize, string? searchTerm = null, bool? isActive = null)
    {
        var query = _context.Partners.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p =>
                p.LegalName.Contains(searchTerm) ||
                (p.TaxId != null && p.TaxId.Contains(searchTerm)) ||
                (p.ContactPerson != null && p.ContactPerson.Contains(searchTerm)) ||
                (p.Email != null && p.Email.Contains(searchTerm)));
        }

        if (isActive.HasValue)
        {
            query = query.Where(p => p.IsActive == isActive.Value);
        }

        var totalCount = await query.CountAsync();

        var partners = await query
            .OrderBy(p => p.LegalName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (partners, totalCount);
    }
}
