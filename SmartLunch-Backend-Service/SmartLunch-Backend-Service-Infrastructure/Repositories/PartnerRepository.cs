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

    public async Task<Partner?> GetByIdAsync(int id)
    {
        return await _context.Partners
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Partner?> GetByIdWithContractsAsync(int id)
    {
        return await _context.Partners
            .Include(p => p.Contracts)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<(List<Partner> Partners, int TotalCount)> GetPartnersAsync(int page, int pageSize, string? searchTerm = null, bool? isActive = null)
    {
        var query = _context.Partners.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(p =>
                p.LegalName.Contains(term) ||
                (p.TaxId != null && p.TaxId.Contains(term)) ||
                (p.BusinessRegistrationNumber != null && p.BusinessRegistrationNumber.Contains(term)) ||
                (p.LegalRepresentative != null && p.LegalRepresentative.Contains(term)) ||
                (p.ContactPerson != null && p.ContactPerson.Contains(term)) ||
                (p.Email != null && p.Email.Contains(term)));
        }

        if (isActive.HasValue)
            query = query.Where(p => p.IsActive == isActive.Value);

        var totalCount = await query.CountAsync();

        var partners = await query
            .OrderBy(p => p.LegalName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (partners, totalCount);
    }

    public async Task<bool> ExistsByTaxIdAsync(string taxId, int? excludePartnerId = null)
    {
        var normalized = taxId.Trim();
        var q = _context.Partners.Where(p => p.TaxId != null && p.TaxId == normalized);
        if (excludePartnerId.HasValue)
            q = q.Where(p => p.Id != excludePartnerId.Value);
        return await q.AnyAsync();
    }

    public async Task<Partner> CreateAsync(Partner partner)
    {
        _context.Partners.Add(partner);
        await _context.SaveChangesAsync();
        return partner;
    }

    public async Task<Partner> UpdateAsync(Partner partner)
    {
        _context.Partners.Update(partner);
        await _context.SaveChangesAsync();
        return partner;
    }

    public async Task<Dictionary<int, string>> GetLegalNamesByIdsAsync(
        IEnumerable<int> partnerIds,
        CancellationToken cancellationToken = default)
    {
        var idList = partnerIds.Distinct().ToList();
        if (idList.Count == 0)
            return new Dictionary<int, string>();

        return await _context.Partners
            .AsNoTracking()
            .Where(p => idList.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => p.LegalName, cancellationToken);
    }
}
