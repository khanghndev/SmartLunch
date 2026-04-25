using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly SmartLunchDBContext _context;

    public TransactionRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<Transaction?> GetByIdAsync(int id)
    {
        return await _context.Transactions
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<(List<Transaction> Transactions, int TotalCount)> GetTransactionsAsync(int page, int pageSize, string? searchTerm = null)
    {
        var query = _context.Transactions.AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(e =>
                (e.Description != null && e.Description.Contains(searchTerm)) ||
                (e.Category != null && e.Category.Contains(searchTerm)) ||
                (e.Method != null && e.Method.Contains(searchTerm)));
        }

        var totalCount = await query.CountAsync();

        var transactions = await query
            .OrderBy(e => e.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (transactions, totalCount);
    }
}
