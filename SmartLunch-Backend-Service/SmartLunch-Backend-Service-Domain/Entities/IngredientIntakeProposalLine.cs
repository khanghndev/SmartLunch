namespace SmartLunch.Backend.Service.Domain.Entities;

public class IngredientIntakeProposalLine
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public int ProposalId { get; set; }
    public int IngredientId { get; set; }
    public decimal Quantity { get; set; }
    public string? LineNote { get; set; }

    public virtual IngredientIntakeProposal Proposal { get; set; } = null!;
    public virtual Ingredient Ingredient { get; set; } = null!;
}
