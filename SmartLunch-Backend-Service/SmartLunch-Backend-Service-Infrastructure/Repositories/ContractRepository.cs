using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.OrganizationMealContractOrders;
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
            .Include(c => c.Organization)
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
                (e.Partner != null && e.Partner.LegalName.Contains(term)) ||
                (e.Organization != null && e.Organization.Name.Contains(term)) ||
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

    public async Task<Contract?> GetPeriodBasedWithExcludedDatesAsync(int contractId, CancellationToken cancellationToken = default)
    {
        return await _context.Contracts
            .Include(c => c.Partner)
            .Include(c => c.Organization)
            .Include(c => c.ExcludedDates)
            .Include(c => c.DailyMealPortions)
            .FirstOrDefaultAsync(
                c => c.Id == contractId &&
                     c.ContractType == OrganizationMealContractTypes.PeriodBased,
                cancellationToken);
    }

    public async Task ReplaceExcludedDatesAsync(
        int contractId,
        IEnumerable<DateOnly> excludedDates,
        CancellationToken cancellationToken = default)
    {
        var existing = await _context.ContractExcludedDates
            .Where(e => e.ContractId == contractId)
            .ToListAsync(cancellationToken);
        _context.ContractExcludedDates.RemoveRange(existing);

        foreach (var d in excludedDates.Distinct())
        {
            _context.ContractExcludedDates.Add(new ContractExcludedDate
            {
                ContractId = contractId,
                ExcludedDate = d,
                CreatedAt = VietnamTime.Now,
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task ReplaceDailyMealPortionsAsync(
        int contractId,
        IEnumerable<ContractDailyMealPortionSource> portions,
        CancellationToken cancellationToken = default)
    {
        var existing = await _context.ContractDailyMealPortions
            .Where(e => e.ContractId == contractId)
            .ToListAsync(cancellationToken);
        _context.ContractDailyMealPortions.RemoveRange(existing);

        foreach (var p in portions)
        {
            _context.ContractDailyMealPortions.Add(new ContractDailyMealPortion
            {
                ContractId = contractId,
                ServiceDate = p.ServiceDate,
                MealCount = p.MealCount,
                CreatedAt = VietnamTime.Now,
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Contract>> GetActivePeriodBasedContractsAsync(CancellationToken cancellationToken = default)
    {
        var today = VietnamTime.Today.ToDateTime(TimeOnly.MinValue);
        return await _context.Contracts
            .Include(c => c.Organization)
            .Include(c => c.ExcludedDates)
            .Include(c => c.DailyMealPortions)
            .Include(c => c.Orders)
            .Where(c =>
                c.ContractType == OrganizationMealContractTypes.PeriodBased &&
                c.Status == ContractStatus.Active &&
                c.StartDate.Date <= today &&
                (c.EndDate == null || c.EndDate.Value.Date >= today) &&
                c.SourceOrderId.HasValue)
            .ToListAsync(cancellationToken);
    }
}
