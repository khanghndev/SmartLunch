using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class PromotionRepository : IPromotionRepository
{
    private readonly SmartLunchDBContext _context;

    public PromotionRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public Task<Promotion?> GetByIdWithTargetsAsync(int id, CancellationToken cancellationToken = default) =>
        _context.Promotions
            .Include(p => p.Targets)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public Task<Promotion?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var normalized = code.Trim();
        return _context.Promotions
            .Include(p => p.Targets)
            .FirstOrDefaultAsync(
                p => p.Code != null && p.Code == normalized,
                cancellationToken);
    }

    public async Task<(List<Promotion> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? searchTerm,
        bool? isActive,
        string? scopeType,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Promotions.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(p =>
                p.Name.Contains(term) ||
                (p.Code != null && p.Code.Contains(term)) ||
                (p.Description != null && p.Description.Contains(term)));
        }

        if (isActive.HasValue)
            query = query.Where(p => p.IsActive == isActive.Value);

        if (!string.IsNullOrWhiteSpace(scopeType))
            query = query.Where(p => p.ScopeType == scopeType.Trim());

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Include(p => p.Targets)
            .OrderByDescending(p => p.Priority)
            .ThenBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public Task<List<Promotion>> GetActiveForEvaluationAsync(DateOnly today, CancellationToken cancellationToken = default) =>
        _context.Promotions
            .Include(p => p.Targets)
            .Where(p =>
                p.IsActive &&
                p.ValidFrom <= today &&
                p.ValidTo >= today)
            .OrderByDescending(p => p.Priority)
            .ThenBy(p => p.Id)
            .ToListAsync(cancellationToken);

    public async Task<bool> ExistsByCodeAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var normalized = code.Trim();
        var query = _context.Promotions.Where(p => p.Code == normalized);
        if (excludeId.HasValue)
            query = query.Where(p => p.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public Task<int> CountTotalApplicationsAsync(int promotionId, CancellationToken cancellationToken = default) =>
        _context.OrderPromotionApplications.CountAsync(a => a.PromotionId == promotionId, cancellationToken);

    public Task<int> CountUserApplicationsAsync(int promotionId, int userId, CancellationToken cancellationToken = default) =>
        _context.OrderPromotionApplications
            .Where(a => a.PromotionId == promotionId && a.Order != null && a.Order.UserId == userId)
            .CountAsync(cancellationToken);

    public async Task<Promotion> CreateAsync(Promotion entity, CancellationToken cancellationToken = default)
    {
        _context.Promotions.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<Promotion> UpdateAsync(Promotion entity, CancellationToken cancellationToken = default)
    {
        _context.Promotions.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task DeleteAsync(Promotion entity, CancellationToken cancellationToken = default)
    {
        _context.Promotions.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task ReplaceTargetsAsync(int promotionId, List<PromotionTarget> targets, CancellationToken cancellationToken = default)
    {
        var existing = await _context.PromotionTargets
            .Where(t => t.PromotionId == promotionId)
            .ToListAsync(cancellationToken);
        _context.PromotionTargets.RemoveRange(existing);

        foreach (var t in targets)
        {
            t.PromotionId = promotionId;
            t.Id = 0;
            _context.PromotionTargets.Add(t);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddApplicationAsync(OrderPromotionApplication application, CancellationToken cancellationToken = default)
    {
        _context.OrderPromotionApplications.Add(application);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
