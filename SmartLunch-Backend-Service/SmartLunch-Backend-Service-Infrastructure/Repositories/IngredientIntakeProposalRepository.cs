using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class IngredientIntakeProposalRepository : IIngredientIntakeProposalRepository
{
    private readonly SmartLunchDBContext _context;

    public IngredientIntakeProposalRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<IngredientIntakeProposal?> GetByIdWithDetailsAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.IngredientIntakeProposals
            .AsNoTracking()
            .Include(p => p.Lines)
            .ThenInclude(l => l.Ingredient)
            .Include(p => p.CreatedByUser)
            .Include(p => p.ReviewedByUser)
            .Include(p => p.ActualIntake)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<IngredientIntakeProposal> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        int? createdByUserIdFilter,
        CancellationToken cancellationToken = default)
    {
        var query = _context.IngredientIntakeProposals.AsNoTracking().AsQueryable();
        if (createdByUserIdFilter.HasValue)
            query = query.Where(p => p.CreatedByUserId == createdByUserIdFilter.Value);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .Include(p => p.CreatedByUser)
            .Include(p => p.ActualIntake)
            .OrderByDescending(p => p.CreatedAt)
            .ThenByDescending(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<IngredientIntakeProposal> CreateAsync(
        IngredientIntakeProposal proposal,
        CancellationToken cancellationToken = default)
    {
        _context.IngredientIntakeProposals.Add(proposal);
        await _context.SaveChangesAsync(cancellationToken);
        return proposal;
    }

    public async Task<IngredientIntakeProposal> ReviewProposalAsync(
        int proposalId,
        int reviewerUserId,
        bool approve,
        string? reviewNote,
        CancellationToken cancellationToken = default)
    {
        var p = await _context.IngredientIntakeProposals
            .FirstOrDefaultAsync(x => x.Id == proposalId, cancellationToken);
        if (p == null)
            throw new KeyNotFoundException($"Proposal {proposalId} was not found.");

        if (p.Status != IntakeProposalStatus.Submitted)
        {
            throw new InvalidOperationException(
                $"Only submitted proposals can be reviewed. Current status: {p.Status}.");
        }

        var note = string.IsNullOrWhiteSpace(reviewNote) ? null : reviewNote.Trim();
        if (note != null && note.Length > 500)
            note = note[..500];

        p.Status = approve ? IntakeProposalStatus.Approved : IntakeProposalStatus.Rejected;
        p.ReviewedByUserId = reviewerUserId;
        p.ReviewedAt = VietnamTime.Now;
        p.ReviewNote = note;
        await _context.SaveChangesAsync(cancellationToken);
        return p;
    }

    public async Task<(IReadOnlyList<IngredientIntakeProposal> Items, int TotalCount)> GetReviewHistoryPagedAsync(
        int page,
        int pageSize,
        int? createdByUserIdFilter,
        CancellationToken cancellationToken = default)
    {
        var query = _context.IngredientIntakeProposals
            .AsNoTracking()
            .Where(p => p.Status != IntakeProposalStatus.Submitted);

        if (createdByUserIdFilter.HasValue)
            query = query.Where(p => p.CreatedByUserId == createdByUserIdFilter.Value);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .Include(p => p.CreatedByUser)
            .Include(p => p.ReviewedByUser)
            .OrderByDescending(p => p.ReviewedAt ?? p.CreatedAt)
            .ThenByDescending(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }
}
