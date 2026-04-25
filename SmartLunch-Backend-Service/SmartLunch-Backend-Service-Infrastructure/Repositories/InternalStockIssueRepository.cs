using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class InternalStockIssueRepository : IInternalStockIssueRepository
{
    private readonly SmartLunchDBContext _context;

    public InternalStockIssueRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<InternalStockIssue?> GetByIdWithLinesAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.InternalStockIssues
            .AsNoTracking()
            .Include(x => x.Lines)
            .ThenInclude(l => l.Ingredient)
            .Include(x => x.CreatedByUser)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<InternalStockIssue> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        DateTime? issuedFromUtc,
        DateTime? issuedToUtcExclusive,
        CancellationToken cancellationToken = default)
    {
        var query = _context.InternalStockIssues.AsNoTracking().AsQueryable();

        if (issuedFromUtc.HasValue)
            query = query.Where(x => x.IssuedAt >= issuedFromUtc.Value);
        if (issuedToUtcExclusive.HasValue)
            query = query.Where(x => x.IssuedAt < issuedToUtcExclusive.Value);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .Include(x => x.CreatedByUser)
            .Include(x => x.Lines)
            .OrderByDescending(x => x.IssuedAt)
            .ThenByDescending(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<InternalStockIssue> CreateAndDeductStockAsync(
        InternalStockIssue issue,
        IReadOnlyList<(int IngredientId, decimal Quantity)> lines,
        CancellationToken cancellationToken = default)
    {
        if (lines.Count == 0)
            throw new ArgumentException("At least one line is required.");

        var ingredientIds = lines.Select(l => l.IngredientId).Distinct().ToList();
        var ingredients = await _context.Ingredients
            .Where(i => ingredientIds.Contains(i.Id))
            .ToDictionaryAsync(i => i.Id, cancellationToken);

        if (ingredients.Count != ingredientIds.Count)
            throw new KeyNotFoundException("One or more ingredients were not found.");

        foreach (var ing in ingredients.Values)
        {
            if (!ing.IsActive)
                throw new InvalidOperationException($"Ingredient '{ing.Name}' is inactive and cannot be issued.");
        }

        await using var tx = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            issue.Lines = lines
                .Select(l => new InternalStockIssueLine
                {

                    IssueId = issue.Id,
                    IngredientId = l.IngredientId,
                    Quantity = l.Quantity
                })
                .ToList();

            _context.InternalStockIssues.Add(issue);

            foreach (var (ingredientId, qty) in lines)
            {
                var inv = await _context.Inventories
                    .FirstOrDefaultAsync(i => i.IngredientId == ingredientId, cancellationToken);
                if (inv == null)
                {
                    throw new InvalidOperationException(
                        $"No inventory record for ingredient {ingredientId}. Create stock before issuing.");
                }

                if (inv.QuantityAvailable < qty)
                {
                    throw new InvalidOperationException(
                        $"Insufficient stock for ingredient {ingredientId}. Available: {inv.QuantityAvailable}, requested: {qty}.");
                }

                inv.QuantityAvailable -= qty;
                inv.LastUpdated = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }

        var reloaded = await GetByIdWithLinesAsync(issue.Id, cancellationToken);
        return reloaded ?? issue;
    }
}
