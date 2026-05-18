using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Domain.Time;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class ContractRepository : IContractRepository
{
    private readonly SmartLunchDBContext _context;

    public ContractRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<Contract?> GetByIdAsync(int id)
    {
        return await _context.Contracts
            .Include(c => c.Partner)
            .Include(c => c.Organization)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<List<Contract>> GetByOrganizationIdsAsync(
        IReadOnlyList<int> organizationIds,
        CancellationToken cancellationToken = default)
    {
        if (organizationIds.Count == 0)
            return new List<Contract>();

        return await _context.Contracts
            .Include(c => c.Partner)
            .Include(c => c.Organization)
            .Where(c => c.OrganizationId.HasValue && organizationIds.Contains(c.OrganizationId.Value))
            .OrderByDescending(c => c.StartDate)
            .Take(50)
            .ToListAsync(cancellationToken);
    }

    public async Task<Contract?> GetActiveForOrganizationAsync(int organizationId, CancellationToken cancellationToken = default)
    {
        return await _context.Contracts
            .Include(c => c.Partner)
            .Where(c =>
                c.OrganizationId == organizationId &&
                c.Status == "active" &&
                c.StartDate.Date <= VietnamTime.Today.ToDateTime(TimeOnly.MinValue) &&
                (c.EndDate == null || c.EndDate.Value.Date >= VietnamTime.Today.ToDateTime(TimeOnly.MinValue)))
            .OrderByDescending(c => c.StartDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<(List<Contract> Contracts, int TotalCount)> GetContractsAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        int? partnerId = null)
    {
        var query = _context.Contracts
            .Include(c => c.Partner)
            .AsQueryable();

        if (partnerId.HasValue)
            query = query.Where(e => e.PartnerId == partnerId.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(e =>
                (e.Description != null && e.Description.Contains(term)) ||
                (e.ContractNumber != null && e.ContractNumber.Contains(term)) ||
                (e.SupplySchedule != null && e.SupplySchedule.Contains(term)) ||
                e.Status.Contains(term));
        }

        var totalCount = await query.CountAsync();

        var contracts = await query
            .OrderByDescending(e => e.StartDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (contracts, totalCount);
    }

    public async Task<bool> ExistsContractNumberForPartnerAsync(int partnerId, string contractNumber, int? excludeContractId = null)
    {
        var n = contractNumber.Trim();
        var q = _context.Contracts.Where(c =>
            c.PartnerId == partnerId &&
            c.ContractNumber != null &&
            c.ContractNumber == n);
        if (excludeContractId.HasValue)
            q = q.Where(c => c.Id != excludeContractId.Value);
        return await q.AnyAsync();
    }

    public async Task<int> CountPartnerPaymentsAsync(int contractId)
    {
        return await _context.PartnerPayments.CountAsync(pp => pp.ContractId == contractId);
    }

    public async Task<Contract> CreateAsync(Contract contract)
    {
        _context.Contracts.Add(contract);
        await _context.SaveChangesAsync();
        return contract;
    }

    public async Task<Contract> UpdateAsync(Contract contract)
    {
        _context.Contracts.Update(contract);
        await _context.SaveChangesAsync();
        return contract;
    }

    public async Task DeleteAsync(Contract contract)
    {
        _context.Contracts.Remove(contract);
        await _context.SaveChangesAsync();
    }
}
