using SmartLunch.Backend.Service.Application.DTOs.Response.IngredientIntake;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Mappings;

public static class IngredientIntakeProposalMapping
{
    public static string? FormatUserDisplayName(User? u)
    {
        if (u == null)
            return null;
        var parts = new[] { u.FirstName, u.LastName }.Where(s => !string.IsNullOrWhiteSpace(s));
        var name = string.Join(' ', parts);
        return string.IsNullOrWhiteSpace(name) ? u.Username : name;
    }

    public static IngredientIntakeProposalSummaryDto ToSummary(IngredientIntakeProposal p)
    {
        return new IngredientIntakeProposalSummaryDto
        {
            Id = p.Id,
            ProposalCode = p.ProposalCode,
            Status = p.Status,
            HeaderNote = p.HeaderNote,
            CreatedByUserId = p.CreatedByUserId,
            CreatedByDisplayName = FormatUserDisplayName(p.CreatedByUser),
            CreatedAt = p.CreatedAt,
            HasActualReceipt = p.ActualIntake != null
        };
    }

    public static IngredientIntakeProposalDetailDto ToDetail(IngredientIntakeProposal p)
    {
        return new IngredientIntakeProposalDetailDto
        {
            Id = p.Id,
            ProposalCode = p.ProposalCode,
            Status = p.Status,
            HeaderNote = p.HeaderNote,
            CreatedByUserId = p.CreatedByUserId,
            CreatedByDisplayName = FormatUserDisplayName(p.CreatedByUser),
            CreatedAt = p.CreatedAt,
            ReviewedByUserId = p.ReviewedByUserId,
            ReviewedByDisplayName = FormatUserDisplayName(p.ReviewedByUser),
            ReviewedAt = p.ReviewedAt,
            ReviewNote = p.ReviewNote,
            HasActualReceipt = p.ActualIntake != null,
            ActualReceiptCode = p.ActualIntake?.ReceiptCode,
            Lines = (p.Lines ?? Array.Empty<IngredientIntakeProposalLine>())
                .OrderBy(l => l.Ingredient.Name)
                .Select(l => new IngredientIntakeProposalLineDto
                {
                    Id = l.Id,
                    IngredientId = l.IngredientId,
                    IngredientName = l.Ingredient.Name,
                    Unit = l.Ingredient.Unit,
                    Quantity = l.Quantity,
                    LineNote = l.LineNote
                })
                .ToList()
        };
    }

    public static IntakeProposalReviewHistoryEntryDto ToReviewHistoryEntry(IngredientIntakeProposal p)
    {
        return new IntakeProposalReviewHistoryEntryDto
        {
            ProposalId = p.Id,
            ProposalCode = p.ProposalCode,
            Status = p.Status,
            HeaderNote = p.HeaderNote,
            CreatedAt = p.CreatedAt,
            CreatedByUserId = p.CreatedByUserId,
            CreatedByDisplayName = FormatUserDisplayName(p.CreatedByUser),
            ReviewedAt = p.ReviewedAt,
            ReviewedByUserId = p.ReviewedByUserId,
            ReviewedByDisplayName = FormatUserDisplayName(p.ReviewedByUser),
            ReviewNote = p.ReviewNote
        };
    }
}
