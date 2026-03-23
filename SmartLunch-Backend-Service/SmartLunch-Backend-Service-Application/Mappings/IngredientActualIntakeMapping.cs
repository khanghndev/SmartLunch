using SmartLunch.Backend.Service.Application.DTOs.Response.IngredientIntake;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Mappings;

public static class IngredientActualIntakeMapping
{
    public static IngredientActualIntakeDetailDto ToDetail(IngredientActualIntake x)
    {
        return new IngredientActualIntakeDetailDto
        {
            Id = x.Id,
            ReceiptCode = x.ReceiptCode,
            ProposalId = x.ProposalId,
            ProposalCode = x.Proposal.ProposalCode,
            CreatedByUserId = x.CreatedByUserId,
            CreatedByDisplayName = IngredientIntakeProposalMapping.FormatUserDisplayName(x.CreatedByUser),
            ReceivedAt = x.ReceivedAt,
            Note = x.Note,
            CreatedAt = x.CreatedAt,
            Lines = (x.Lines ?? Array.Empty<IngredientActualIntakeLine>())
                .OrderBy(l => l.Ingredient.Name)
                .Select(l => new IngredientActualIntakeLineDto
                {
                    Id = l.Id,
                    IngredientId = l.IngredientId,
                    IngredientName = l.Ingredient.Name,
                    Unit = l.Ingredient.Unit,
                    Quantity = l.Quantity
                })
                .ToList()
        };
    }
}
