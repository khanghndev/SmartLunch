using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class ContractRepository : IContractRepository
{
    private readonly SmartLunchDBContext _context;

    public ContractRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<Contract?> GetByIdAsync(Guid id)
    {
        return await _context.Contracts
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<(List<Contract> Contracts, int TotalCount)> GetContractsAsync(int page, int pageSize, string? searchTerm = null)
    {
        var query = _context.Contracts.AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(e =>
                (e.Description != null && e.Description.Contains(searchTerm)) ||
                e.Status.Contains(searchTerm));
        }

        var totalCount = await query.CountAsync();

        var contracts = await query
            .OrderBy(e => e.StartDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (contracts, totalCount);
    }
}
