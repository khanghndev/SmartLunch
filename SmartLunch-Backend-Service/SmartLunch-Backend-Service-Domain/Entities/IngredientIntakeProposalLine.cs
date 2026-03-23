namespace SmartLunch.Backend.Service.Domain.Entities;

public class IngredientIntakeProposalLine
{
    public Guid Id { get; set; }
    public Guid ProposalId { get; set; }
    public Guid IngredientId { get; set; }
    public decimal Quantity { get; set; }
    public string? LineNote { get; set; }

    public virtual IngredientIntakeProposal Proposal { get; set; } = null!;
    public virtual Ingredient Ingredient { get; set; } = null!;
}
