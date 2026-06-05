using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class ContractWeeklySelectionRepository : IContractWeeklySelectionRepository
{
    private readonly SmartLunchDBContext _context;

    public ContractWeeklySelectionRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<ContractWeeklySelection?> GetByContractAndWeekAsync(
        int contractId,
        DateOnly weekMonday,
        bool includeItems = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<ContractWeeklySelection> query = _context.ContractWeeklySelections;
        if (includeItems)
        {
            query = query
                .Include(w => w.Items)
                .ThenInclude(i => i.Dish);
        }

        return await query.FirstOrDefaultAsync(
            w => w.ContractId == contractId && w.WeekMonday == weekMonday,
            cancellationToken);
    }

    public async Task<List<ContractWeeklySelection>> GetAllByContractIdAsync(
        int contractId,
        bool includeItems = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<ContractWeeklySelection> query = _context.ContractWeeklySelections;
        if (includeItems)
        {
            query = query
                .Include(w => w.Items)
                .ThenInclude(i => i.Dish);
        }

        return await query
            .Where(w => w.ContractId == contractId)
            .OrderBy(w => w.WeekMonday)
            .ToListAsync(cancellationToken);
    }

    public async Task<(int TotalWeeks, int FilledWeeks)> GetProgressAsync(
        int contractId,
        CancellationToken cancellationToken = default)
    {
        var rows = await _context.ContractWeeklySelections
            .Where(w => w.ContractId == contractId)
            .Select(w => w.Status)
            .ToListAsync(cancellationToken);

        var total = rows.Count;
        var filled = rows.Count(s => ContractWeeklySelectionStatuses.IsFilled(s));
        return (total, filled);
    }

    public async Task AddAsync(ContractWeeklySelection selection, CancellationToken cancellationToken = default)
    {
        await _context.ContractWeeklySelections.AddAsync(selection, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
