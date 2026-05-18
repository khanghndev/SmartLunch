using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class IngredientActualIntakeRepository : IIngredientActualIntakeRepository
{
    private readonly SmartLunchDBContext _context;

    public IngredientActualIntakeRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<IngredientActualIntake> CreateFromApprovedProposalAsync(
        int proposalId,
        int actorUserId,
        bool actorIsElevated,
        DateTime receivedAtUtc,
        string? note,
        CancellationToken cancellationToken = default)
    {
        if (receivedAtUtc.Kind == DateTimeKind.Unspecified)
            receivedAtUtc = DateTime.SpecifyKind(receivedAtUtc, DateTimeKind.Utc);

        await using var tx = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var hasReceipt = await _context.IngredientActualIntakes
                .AnyAsync(i => i.ProposalId == proposalId, cancellationToken);
            if (hasReceipt)
                throw new InvalidOperationException("This proposal already has an actual intake receipt.");

            var proposal = await _context.IngredientIntakeProposals
                .Include(p => p.Lines)
                .FirstOrDefaultAsync(p => p.Id == proposalId, cancellationToken);

            if (proposal == null)
                throw new KeyNotFoundException($"Proposal {proposalId} was not found.");

            if (proposal.Status != IntakeProposalStatus.Approved)
                throw new InvalidOperationException(
                    $"Only approved proposals can be received. Current status: {proposal.Status}.");

            if (!actorIsElevated && proposal.CreatedByUserId != actorUserId)
                throw new InvalidOperationException("You may only create receipts for your own proposals.");

            if (proposal.Lines == null || proposal.Lines.Count == 0)
                throw new InvalidOperationException("Proposal has no lines.");

            var ingredientIds = proposal.Lines.Select(l => l.IngredientId).Distinct().ToList();
            var ingredients = await _context.Ingredients
                .Where(i => ingredientIds.Contains(i.Id))
                .ToDictionaryAsync(i => i.Id, cancellationToken);

            if (ingredients.Count != ingredientIds.Count)
                throw new InvalidOperationException("One or more ingredients on the proposal no longer exist.");

            foreach (var ing in ingredients.Values)
            {
                if (!ing.IsActive)
                    throw new InvalidOperationException($"Ingredient '{ing.Name}' is inactive; cannot receive stock.");
            }

            var receiptCode =
                $"PNK-{VietnamTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";

            var intake = new IngredientActualIntake
            {
                ReceiptCode = receiptCode,
                ProposalId = proposalId,
                CreatedByUserId = actorUserId,
                ReceivedAt = receivedAtUtc,
                Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim().Length > 500 ? note.Trim()[..500] : note.Trim(),
                CreatedAt = VietnamTime.Now
            };

            foreach (var pl in proposal.Lines)
            {
                intake.Lines.Add(new IngredientActualIntakeLine
                {
                    IngredientId = pl.IngredientId,
                    Quantity = pl.Quantity
                });

                var inv = await _context.Inventories
                    .FirstOrDefaultAsync(i => i.IngredientId == pl.IngredientId, cancellationToken);
                if (inv == null)
                {
                    _context.Inventories.Add(new Inventory
                    {
                        IngredientId = pl.IngredientId,
                        QuantityAvailable = pl.Quantity,
                        ReorderLevel = null,
                        LastUpdated = VietnamTime.Now
                    });
                }
                else
                {
                    inv.QuantityAvailable += pl.Quantity;
                    inv.LastUpdated = VietnamTime.Now;
                }
            }

            proposal.Status = IntakeProposalStatus.Fulfilled;
            _context.IngredientActualIntakes.Add(intake);
            await _context.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }

        var reloaded = await _context.IngredientActualIntakes
            .AsNoTracking()
            .Include(i => i.Lines)
            .ThenInclude(l => l.Ingredient)
            .Include(i => i.Proposal)
            .Include(i => i.CreatedByUser)
            .FirstAsync(i => i.ProposalId == proposalId, cancellationToken);

        return reloaded;
    }
}
